using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EchoBot.Model;

namespace EchoBot.Repository
{
    public class DegreeCourseRepository : IDegreeCourseRepository, IDisposable
    {
        public IEnumerable<DegreeCourse> GetDegreeCourses()
        {
            throw new NotImplementedException();
        }

        public void InsertDegreeCourse(DegreeCourse degreeCourse)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                //if (disposing)
                    //context.Dispose(); //TODO
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
