using Parrot.Infrastructure;

namespace Parrot.Nodes
{
    using System.Linq;

    public abstract class AbstractNode
    {
        private LocalsStack _stack;
        public object Model { get; set; }

        public abstract bool IsTerminal
        {
            get;
        }

        public AbstractNode SetModel(object model)
        {
            Model = model;

            return this;
        }

        public AbstractNode SetStack(LocalsStack stack)
        {
            _stack = stack;

            return this;
        }
        protected object GetModelValue(string parameterName)
        {
            return GetModelValue(Model, parameterName);
        }

        protected object GetModelValue(object localModel, string parameterName)
        {
            string[] parameters = parameterName.Split(".".ToCharArray());
            
            object modelToCheck = localModel;

            if (parameterName == "this")
            {
                return localModel;
            }

            //check the local stack
            if (_stack != null)
            {
                object result;
                if (_stack.Get(parameterName, out result))
                {
                    return result;
                }
            }


            if (localModel != null)
            {
                var pi = localModel.GetType().GetProperty(parameters[0]);
                if (pi != null)
                {
                    var tempObject = pi.GetValue(localModel, null);

                    if (parameters.Length == 1)
                    {
                        return tempObject;
                    }

                    return GetModelValue(tempObject, string.Join(".", parameters.Skip(1)));
                }
            }

            return null;
        }
    }
}