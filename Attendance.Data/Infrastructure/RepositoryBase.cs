using EntityFramework.BulkInsert.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;


namespace Attendance.Data.Infrastructure
{
    public abstract class RepositoryBase<T> where T : class
    {
        private ApplicationEntities dataContext;
        private readonly IDbSet<T> dbset;


        protected RepositoryBase(IDatabaseFactory databaseFactory)
        {
            DatabaseFactory = databaseFactory;
            dbset = DataContext.Set<T>();
        }

        protected IDatabaseFactory DatabaseFactory
        {
            get;
            private set;
        }

        protected ApplicationEntities DataContext
        {
            get { return dataContext ?? (dataContext = DatabaseFactory.Get()); }
        }
        public virtual void Add(T entity)
        {
            dbset.Add(entity);
        }
        public virtual void Update(T entity)
        {
            dbset.Attach(entity);
            dataContext.Entry(entity).State = EntityState.Modified;
        }
        public virtual void Delete(T entity)
        {
            dbset.Remove(entity);
        }
   
        public virtual void Delete(Expression<Func<T, bool>> where)
        {
            IEnumerable<T> objects = dbset.Where<T>(where).AsEnumerable();
            foreach (T obj in objects)
                dbset.Remove(obj);
        }
        public virtual T GetById(long id)
        {
            return dbset.Find(id);
        }
        public virtual T GetById(params object[] keyValues)
        {
            return dbset.Find(keyValues);
        }
        public virtual T GetById(string id)
        {
            return dbset.Find(id);
        }

        public virtual T GetById(Guid id)
        {
            return dbset.Find(id);
        }

        public virtual IEnumerable<T> GetAll()
        {
            return dbset.ToList();
        }

        public virtual IEnumerable<T> GetMany(Expression<Func<T, bool>> where)
        {
            return dbset.Where(where).ToList();
        }

        public T Get(Expression<Func<T, bool>> where)
        {
            return dbset.Where(where).FirstOrDefault<T>();
        }

        public virtual void BulkSave(List<T> entities)
        {

            using (var transactionScope = new TransactionScope())
            {
                using (var ctx = new ApplicationEntities())
                {
                    // some stuff in dbcontext
                    ctx.BulkInsert(entities);
                    ctx.SaveChanges();
                    //ctx.Commit();
                    transactionScope.Complete();
                }
            }
        }


        //        using (var ctx = new SchoolDBEntities())
        //        {
        //            var idParam = new SqlParameter
        //            {
        //                ParameterName = "StudentID",
        //                Value = 1
        //            };
        //            //Get student name of string type
        //            var courseList = ctx.Database.SqlQuery<Course>("exec GetCoursesByStudentId @StudentId ", idParam).ToList<Course>();
        //            
        //            //Or can call SP by following way
        //            //var courseList = ctx.Courses.SqlQuery("exec GetCoursesByStudentId @StudentId ", idParam).ToList<Course>();
        //
        //            foreach (Course cs in courseList)
        //                Console.WriteLine("Course Name: {0}",cs.CourseName);
        //        }



        //public virtual void ExecuteSP(long p1, int p2, int p3)
        //{

        //    using (var transactionScope = new TransactionScope())
        //    {
        //        // some stuff in dbcontext
        //        //("CalculateMyTeamMatchScore @MatchScheduleIdIn, @TournamentIdIn", parameters: new[] { p1, p2 });

        //        try
        //        {


        //            //                    var rowsAffected = dataContext.Database.ExecuteSqlCommand(
        //            //                        "CalculateMyTeamMatchScore @MatchScheduleIdIn, @TournamentIdIn", parameters: new[] { 15, 14});
        //            //ctx.SaveChanges();
        //            var rowsAffected = dataContext.Database.ExecuteSqlCommand(
        //                "CalculateMyTeamMatchScore @MatchScheduleIdIn, @TournamentIdIn, @IsScoreComplete",
        //                new SqlParameter("MatchScheduleIdIn", p1),
        //                new SqlParameter("TournamentIdIn", p2),
        //                new SqlParameter("IsScoreComplete", p3));

        //            transactionScope.Complete();
        //        }
        //        catch (Exception oException)
        //        {
        //            string message = oException.Message;
        //        }
        //    }
        //}


        public virtual void ExecuteSP(string procName, Dictionary<string, object> keyValuePairs)
        {
            int size = 0;
            size = keyValuePairs.Keys.Count;
            SqlParameter[] parameters = new SqlParameter[size];
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            foreach (string key in keyValuePairs.Keys)
            {
                SqlParameter sqlParameter = new SqlParameter(key, keyValuePairs[key]);
                sqlParameters.Add(sqlParameter);

            }
            parameters = sqlParameters.ToArray();

            using (var transactionScope = new TransactionScope())
            {

                try
                {
                    var rowsAffected = dataContext.Database.ExecuteSqlCommand(
                        procName, parameters);
                    transactionScope.Complete();
                }
                catch (Exception oException)
                {
                    string message = oException.Message;
                }
            }
        }


    }
}
