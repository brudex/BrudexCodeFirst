using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brudex.CodeFirst
{
    public enum MigrationOptions
    {
       MigrateOnce, RecreateEveryTime,  RecreateOnlyChangedModels , AlterOnModelChanges
    }
}   
