#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion

namespace Pic.Factory2D
{
    public interface IToolInterface
    {
        Box2D BoundingBox { get; }
        bool ReflectionX { get; set; }
        bool ReflectionY { get; set; }
        bool ShowCotationsAuto { get; set; }
        bool ShowCotationsCode { get; set; }
        bool ShowAxes { get; set; }

        bool IsPluginViewer { get; }
        bool HasDependancies { get; }
        bool IsSupportingAutomaticFolding { get; }
    }
}
