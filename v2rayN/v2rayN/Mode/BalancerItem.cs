namespace v2rayN.Mode
{
    [Serializable]
    public class BalancerItem
    {
        public string tag { get; set; }
        public string[] selector { get; set; }

        public BalancerStrategyItem strategy { get; set; }

    }
    [Serializable]
    public class BalancerStrategyItem
    {
        public string type { get; set; }
    }

}
