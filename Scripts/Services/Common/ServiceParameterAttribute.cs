namespace Grate.Services{
    [System.AttributeUsage(System.AttributeTargets.Property)]  
    public class ServiceParameterAttribute : System.Attribute  
    {  
        public string Tag {get; private set;}
        public ServiceParameterAttribute(string tag) { Tag = tag; }  
    }  

    [System.AttributeUsage(System.AttributeTargets.Parameter)]  
    public class FromParametersAttribute : System.Attribute  
    {  
        public string Tag {get; private set;}
        public FromParametersAttribute(string tag) { Tag = tag; }  
    }  
}
