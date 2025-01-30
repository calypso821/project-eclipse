using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eclipse.Source.Engine.Core.Entity
{
    internal abstract class Entity
    {
        // TODO: Move GameObject logic to Enitity (

        // BaseObject/Entity
        //├── Common Properties:
        //│   ├── ID/Name
        //│   ├── Active/Enabled state
        //│   ├── Parent/Child relationships
        //│   ├── Component management
        //│   └── Base transform data
        //│
        //├── GameObject
        //│   ├── Game-specific properties
        //│   ├── GameDirtyFlags
        //│   └── Scene-specific methods
        //│
        //└── UIObject
        //    ├── UI-specific properties
        //    ├── UIDirtyFlags
        //    └── Canvas-specific methods
    }
}
