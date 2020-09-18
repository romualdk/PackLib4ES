﻿#region Using directives
using System;
using Sharp3D.Math.Core;
#endregion

namespace Pic.Factory2D
{
    public class PicVisitorTransform : PicFactoryVisitor, IDisposable
    {
        #region Public Constructor
        public PicVisitorTransform(Transform2D transf)  { Transform = transf; }
        #endregion

        #region PicFactoryVisitor overrides
        public override void Initialize(PicFactory factory) { }
        public override void ProcessEntity(PicEntity entity)
        {
            PicTypedDrawable drawable = (PicTypedDrawable)entity;
            if (null != drawable)
                drawable.Transform(Transform);
        }
        public override void Finish() { }
        public void Dispose()  {}
        #endregion

        #region Private fields
        private Transform2D Transform { get; set; }
        #endregion
    }
}
