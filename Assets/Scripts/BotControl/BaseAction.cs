using System;

namespace Bot
{
    public abstract class BaseAction
    {
        public Action onComplete;

        protected Controller c;

        public BaseAction(Controller controller)
        {
            c = controller;
        }

        public abstract void Update();
    }
}