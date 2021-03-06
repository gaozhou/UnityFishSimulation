﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityTools;
using UnityTools.Algorithm;
using UnityTools.Debuging;
using UnityTools.Math;

namespace UnityFishSimulation
{
    [Serializable]
    public class FishLogger
    {
        [Serializable]
        public class Data
        {
            internal protected DiscreteFunction<float, float3> trajectory = new DiscreteFunction<float, float3>();
            internal protected DiscreteFunction<float, float3> velocity = new DiscreteFunction<float, float3>();
            internal protected DiscreteFunction<float, float3> direction = new DiscreteFunction<float, float3>();
            internal protected int currentIndex;
        }

        public Data LogData=>this.logData;
        protected Data logData = new Data();
        protected float sampleInterval = 0.5f;
        protected float current = 0;
        public void Log(FishModelData bodyModel, float current)
        {
            if(current > this.current)
            {
                this.logData.trajectory.Append(current, bodyModel.Head.Position);
                this.logData.velocity.Append(current, bodyModel.Velocity);
                this.logData.direction.Append(current, bodyModel.Direction);
                this.current += this.sampleInterval;
            }
        }
    }
    
    [Serializable]
    public class FishSimulator : SimulatorMono
    {
        /*public static ControllerProblem FishControllerProblem => fishSimulator.problem as ControllerProblem;
        public static FishSimulator Instance => fishSimulator;
        protected static FishSimulator fishSimulator = new FishSimulator(new ControllerProblem(), new Delta());*/

        [Serializable]
        public class ControllerProblem: IProblem
        {
            public LinkedList<FishController> CurrentQueue => this.fishControllers;
            protected LinkedList<FishController> fishControllers = new LinkedList<FishController>();

            public void AddController(FishController controller)
            {
                LogTool.AssertIsFalse(this.CurrentQueue.Contains(controller));
                this.CurrentQueue.AddLast(controller);
            }
        }    
        [Serializable]
        public class Solution : ISolution
        {

        }

        [Serializable]
        public class Delta : IDelta
        {
            protected const float dt = 0.055f;
            public float current;
            public float local;
            public float deltaTime;
            public void Reset()
            {
                this.current = 0;
                this.local = 0;
            }

            public void Step()
            {
                this.current += Time.deltaTime;
                this.deltaTime = Time.deltaTime;
            }
        }

        public void OnInit(
            IProblem problem, 
            IDelta dt, 
            IterationAlgorithmMode mode = IterationAlgorithmMode.FullStep) 
        {
            this.Init(problem,dt,mode);
        }     

        public override bool IsSolutionAcceptable(ISolution solution)
        {
            return false;
        }

        public override ISolution Solve(IProblem problem)
        {
            var d = this.dt as Delta;
            var s = this.CurrentSolution as Solution;
            var p = problem as ControllerProblem;
            foreach (var c in p.CurrentQueue)
            {
                c.MainUpdate(d, s);
            }

            return this.CurrentSolution;
        }

        public void ResetAndRun()
        {
            var p = problem as ControllerProblem;
            foreach (var c in p.CurrentQueue)
            {
                c.Reset();
            }

            this.Reset();
            this.TryToRun();
        }
    }
}