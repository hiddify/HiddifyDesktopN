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
    }

    
    public interface BalancerStrategySettings{}

    [Serializable]
    public class OptimalBalancerStrategySetting: BalancerStrategySettings
    {
        public int timeout { get; set; } = 10000;
        public int interval { get; set; } = 30000;
        public string url { get; set; } = "https://about.google";
        public int count { get; set; } = 3;
    }
}
