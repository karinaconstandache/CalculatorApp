using System;

namespace CalculatorApp
{
    public class AppSettings
    {
        // Whether digit grouping is enabled in Standard mode
        public bool IsDigitGroupingEnabled { get; set; }

        // Last mode used ("Standard" or "Programmer")
        public string LastMode { get; set; }

        // In Programmer mode, the last selected base (e.g. 2, 8, 10, 16)
        public int LastBase { get; set; }
    }
}

