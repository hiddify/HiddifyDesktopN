namespace v2rayN.Mode
{
    [Serializable]
    public class BalancerItem
    {
        public string tag { get; set; }
        public string[] selector { get; set; }

        public BalancerStrategyItem strategy { get; set; }
        public BalancerStrategySettings optimalSettings { get; set; }

    }
    [Serializable]
    public class BalancerStrategyItem
    {
        public string type { get; set; }
        public BalancerStrategySettings settings { get; set; }
    }

    
    public interface BalancerStrategySettings{}

    [Serializable]
    public class OptimalBalancerStrategySetting: BalancerStrategySettings
    {
        public int timeout { get; set; } = 10000;
        public int interval { get; set; } = 30000;
        public string url { get; set; } = "https://about.google";
        public int count { get; set; } = 3;
        public bool accept_little_diff { get; set; } = true;
        public bool load_balancing { get; set; } = false;
        public double diff_percent { get; set; } = 0.5;

    }
}
