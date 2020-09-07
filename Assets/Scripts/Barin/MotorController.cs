﻿using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace UnityFishSimulation
{
    public class MotorController
    {

    }

    public abstract class MuscleMC : MotorController
    {
        public class Parameter
        {
            public readonly float2 aMinMax = new float2(0, 1);
            public readonly float2 fMinMax = new float2(0, 1);
            public float amplitude;
            public float frequency = 1;
        }
        protected FishActivationData activationData;
        protected Dictionary<Spring.Type, Parameter> muscleControlParamters;
        protected abstract List<Spring.Type> GetSprtingTypes();
        protected abstract string FileName { get; }

        public FishActivationData ActivationData => this.activationData;
        public abstract Parameter GetParameter(Spring.Type type);

        public MuscleMC()
        {
            this.muscleControlParamters = new Dictionary<Spring.Type, Parameter>();
            var types = this.GetSprtingTypes();
            foreach (var t in types)
            {
                this.muscleControlParamters.Add(t, new Parameter());
            }

            this.activationData = FishActivationData.Load(this.FileName);
        }
    }

    public class SwimMC : MuscleMC
    {
        protected float speed = 1;
        protected override string FileName => "Swimming";
        protected override List<Spring.Type> GetSprtingTypes()
        {
            return new List<Spring.Type>() { Spring.Type.MuscleMiddle, Spring.Type.MuscleBack };
        }
        public override Parameter GetParameter(Spring.Type type)
        {
            //convert from speed to parameter
            return this.muscleControlParamters.ContainsKey(type)?this.muscleControlParamters[type]: new Parameter();
        }

    }

    public class TurnMC : MuscleMC
    {
        protected Dictionary<int, Parameter> turnAnlgeMap;
        protected float angle = 0;
        protected override string FileName => "Turn";
        protected override List<Spring.Type> GetSprtingTypes()
        {
            return new List<Spring.Type>() { Spring.Type.MuscleFront, Spring.Type.MuscleMiddle };
        }

        public override Parameter GetParameter(Spring.Type type)
        {
            //convert from speed to parameter
            return default;
        }
        public TurnMC() : base()
        {
            this.turnAnlgeMap = new Dictionary<int, Parameter>();
        }
    }

    public class GlideMC : MuscleMC
    {
        protected float time = 0;
        protected override string FileName => "Glide";
        protected override List<Spring.Type> GetSprtingTypes()
        {
            return new List<Spring.Type>() { Spring.Type.MuscleFront, Spring.Type.MuscleMiddle, Spring.Type.MuscleBack };
        }

        public override Parameter GetParameter(Spring.Type type)
        {
            //convert from speed to parameter
            return default;
        }
    }

    public class BalanceMC : MotorController
    {
        protected float3 worldUp = new float3(0, 1, 0);
        protected float leftFin;
        protected float rightFin;
    }
}
