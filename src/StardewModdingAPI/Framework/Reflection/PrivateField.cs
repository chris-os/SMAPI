﻿using System;
using System.Reflection;

namespace StardewModdingAPI.Framework.Reflection
{
    /// <summary>A private field obtained through reflection.</summary>
    /// <typeparam name="TValue">The field value type.</typeparam>
    internal class PrivateField<TValue> : IPrivateField<TValue>
    {
        /*********
        ** Properties
        *********/
        /// <summary>The type that has the field.</summary>
        private readonly Type ParentType;

        /// <summary>The object that has the instance field (if applicable).</summary>
        private readonly object Parent;

        /// <summary>The display name shown in error messages.</summary>
        private string DisplayName => $"{this.ParentType.FullName}::{this.FieldInfo.Name}";


        /*********
        ** Accessors
        *********/
        /// <summary>The reflection metadata.</summary>
        public FieldInfo FieldInfo { get; }


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="parentType">The type that has the field.</param>
        /// <param name="obj">The object that has the instance field (if applicable).</param>
        /// <param name="field">The reflection metadata.</param>
        /// <param name="isStatic">Whether the field is static.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="parentType"/> or <paramref name="field"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="obj"/> is null for a non-static field, or not null for a static field.</exception>
        public PrivateField(Type parentType, object obj, FieldInfo field, bool isStatic)
        {
            // validate
            if (parentType == null)
                throw new ArgumentNullException(nameof(parentType));
            if (field == null)
                throw new ArgumentNullException(nameof(field));
            if (isStatic && obj != null)
                throw new ArgumentException("A static field cannot have an object instance.");
            if (!isStatic && obj == null)
                throw new ArgumentException("A non-static field must have an object instance.");

            // save
            this.ParentType = parentType;
            this.Parent = obj;
            this.FieldInfo = field;
        }

        /// <summary>Get the field value.</summary>
        public TValue GetValue()
        {
            try
            {
                return (TValue)this.FieldInfo.GetValue(this.Parent);
            }
            catch (InvalidCastException)
            {
                throw new InvalidCastException($"Can't convert the private {this.DisplayName} field from {this.FieldInfo.FieldType.FullName} to {typeof(TValue).FullName}.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't get the value of the private {this.DisplayName} field", ex);
            }
        }

        /// <summary>Set the field value.</summary>
        //// <param name="value">The value to set.</param>
        public void SetValue(TValue value)
        {
            try
            {
                this.FieldInfo.SetValue(this.Parent, value);
            }
            catch (InvalidCastException)
            {
                throw new InvalidCastException($"Can't assign the private {this.DisplayName} field a {typeof(TValue).FullName} value, must be compatible with {this.FieldInfo.FieldType.FullName}.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't set the value of the private {this.DisplayName} field", ex);
            }
        }
    }
}
