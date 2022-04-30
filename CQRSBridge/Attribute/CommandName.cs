namespace CQRSBridge.Attribute
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class CommandName : System.Attribute
    {
        private string _command;
        public CommandName(string command)
        {

            _command = command;

        }

        public virtual string Command
        {
            get { return _command; }
        }

    }

}
