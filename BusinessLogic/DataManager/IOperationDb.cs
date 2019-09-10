using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;

namespace BusinessLogic.DataManager
{
    public interface IOperationDb
    {
        IQueryable<T> GetModels<T>() where T : class;
        IQueryable<T> GetModels<T>(Expression<Func<T, bool>> predicate) where T : class;
        void CreateModel<T>(T model) where T : class;
    }
}