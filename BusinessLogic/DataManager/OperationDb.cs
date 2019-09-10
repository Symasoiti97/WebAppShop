using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mime;
using DataBase;
using Z.EntityFramework.Plus;

namespace BusinessLogic.DataManager
{
    public class OperationDb : IOperationDb
    {
        private readonly ApplicationContext _db;
        
        public OperationDb(ApplicationContext db)
        {
            _db = db;
        }

        public IQueryable<T> GetModels<T>() where T : class
        {
            return _db.Set<T>();
        }

        public IQueryable<T> GetModels<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return _db.Set<T>().Where(predicate);
        }

        public void CreateModel<T>(T model) where T : class
        {
            _db.Set<T>().Add(model);
            _db.SaveChanges();
        }

//        public void RemoveModel<T>(T model) where T : class
//        {
//            _db.Set<T>().Remove(model);
//            _db.SaveChanges();
//        }
//
//        public void UpdateModel<T>(T model) where T : class
//        {
//            _db.SaveChanges();
//        }
    }
}