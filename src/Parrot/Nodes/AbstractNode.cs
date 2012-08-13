namespace Parrot.Nodes
{
    using System.Linq;

    public abstract class AbstractNode
    {
        //public object Model { get; set; }

        public abstract bool IsTerminal
        {
            get;
        }

        //public AbstractNode SetModel(object model)
        //{
        //    Model = model;

        //    return this;
        //}

        //protected object GetModelValue(string parameterName)
        //{
        //    return GetModelValue(Model, parameterName);
        //}

        //protected object GetModelValue(object localModel, string parameterName)
        //{
        //    string[] parameters = parameterName.Split(".".ToCharArray());
            
        //    object modelToCheck = localModel;

        //    if (parameterName == "this")
        //    {
        //        return localModel;
        //    }

        //    if (localModel != null)
        //    {
        //        var pi = localModel.GetType().GetProperty(parameters[0]);
        //        if (pi != null)
        //        {
        //            var tempObject = pi.GetValue(localModel, null);

        //            if (parameters.Length == 1)
        //            {
        //                return tempObject;
        //            }

        //            return GetModelValue(tempObject, string.Join(".", parameters.Skip(1)));
        //        }
        //    }

        //    return null;
        //}
    }
}