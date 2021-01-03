using System;
using UnityEngine;
using UnityEngine.Events;

namespace Midity.Playable
{
    //
    // ControlAction class that invokes a property setter implemented in a
    // given target component.
    //
    internal abstract class ControlAction
    {
        public static ControlAction
            CreateAction(object target, string propertyName)
        {
            var type = target?.GetType().GetProperty(propertyName)?.PropertyType;

            if (type == typeof(float))
                return new FloatAction(target, propertyName);

            if (type == typeof(Vector3))
                return new Vector3Action(target, propertyName);

            if (type == typeof(Quaternion))
                return new QuaternionAction(target, propertyName);

            if (type == typeof(Color))
                return new ColorAction(target, propertyName);

            return null;
        }

        public abstract void Invoke(Vector4 param);

        protected static UnityAction<T>
            GetPropertySetter<T>(object target, string propertyName)
        {
            return (UnityAction<T>) Delegate.CreateDelegate
                (typeof(UnityAction<T>), target, "set_" + propertyName);
        }
    }

    internal class FloatAction : ControlAction
    {
        public UnityAction<float> action;

        public FloatAction(object target, string propertyName)
        {
            action = GetPropertySetter<float>(target, propertyName);
        }

        public override void Invoke(Vector4 param)
        {
            action(param.x);
        }
    }

    internal class Vector3Action : ControlAction
    {
        public UnityAction<Vector3> action;

        public Vector3Action(object target, string propertyName)
        {
            action = GetPropertySetter<Vector3>(target, propertyName);
        }

        public override void Invoke(Vector4 param)
        {
            action(param);
        }
    }

    internal class QuaternionAction : ControlAction
    {
        public UnityAction<Quaternion> action;

        public QuaternionAction(object target, string propertyName)
        {
            action = GetPropertySetter<Quaternion>(target, propertyName);
        }

        public override void Invoke(Vector4 param)
        {
            action(Quaternion.Euler(param));
        }
    }

    internal class ColorAction : ControlAction
    {
        public UnityAction<Color> action;

        public ColorAction(object target, string propertyName)
        {
            action = GetPropertySetter<Color>(target, propertyName);
        }

        public override void Invoke(Vector4 param)
        {
            action(param);
        }
    }
}