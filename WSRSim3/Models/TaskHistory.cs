//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WSRSim3.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TaskHistory
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public int StatussId { get; set; }
        public System.DateTime Date { get; set; }
    
        public virtual Task Task { get; set; }
        public virtual TaskStatus TaskStatus { get; set; }
    }
}