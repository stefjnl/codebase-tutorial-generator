using System.Runtime.Serialization;

namespace DotNetTutorialGenerator.Core.Exceptions
{
    [Serializable]
    public class TutorialGenerationException : Exception
    {
        public string Phase { get; }
        public Dictionary<string, object> Context { get; }

        public TutorialGenerationException()
        {
            Phase = string.Empty;
            Context = new Dictionary<string, object>();
        }

        public TutorialGenerationException(string message) : base(message)
        {
            Phase = string.Empty;
            Context = new Dictionary<string, object>();
        }

        public TutorialGenerationException(string message, Exception innerException) : base(message, innerException)
        {
            Phase = string.Empty;
            Context = new Dictionary<string, object>();
        }

        public TutorialGenerationException(string message, string phase) : base(message)
        {
            Phase = phase;
            Context = new Dictionary<string, object>();
        }

        public TutorialGenerationException(string message, string phase, Exception innerException) : base(message, innerException)
        {
            Phase = phase;
            Context = new Dictionary<string, object>();
        }

        public TutorialGenerationException(string message, string phase, Dictionary<string, object> context) : base(message)
        {
            Phase = phase;
            Context = context ?? new Dictionary<string, object>();
        }

        public TutorialGenerationException(string message, string phase, Dictionary<string, object> context, Exception innerException) : base(message, innerException)
        {
            Phase = phase;
            Context = context ?? new Dictionary<string, object>();
        }

        protected TutorialGenerationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Phase = info.GetString(nameof(Phase)) ?? string.Empty;
            Context = (Dictionary<string, object>)info.GetValue(nameof(Context), typeof(Dictionary<string, object>))!;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(Phase), Phase);
            info.AddValue(nameof(Context), Context);
        }
    }
}
