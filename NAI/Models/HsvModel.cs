namespace NAI.Models
{
    public class HsvModel
    {
        public ModelItem Hue { get; set; }
        public ModelItem Saturation { get; set; }
        public ModelItem Value { get; set; }

        public HsvModel()
        {
            Hue = new ModelItem();
            Saturation = new ModelItem();
            Value = new ModelItem();
        }
    }
}
