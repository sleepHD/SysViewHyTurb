using System;
using System.Collections.Generic;
using System.Text;

namespace SysViewHyTurb
{
    public class DataRepo
    {
        public float[] InputRegisters { get { return this.inputRegisters;} }
        private float[] inputRegisters = new float[256];
    }
}
