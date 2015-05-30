using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using NHibernate;

namespace StrongApi
{
    public class NhApiController : ApiController
    {
        protected readonly ISession Session;
        protected readonly ITransaction Transaction;

        public NhApiController(ISession session)
        {
            Session = session;
            Transaction = Session.BeginTransaction();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Transaction.Dispose();
                Session.Dispose();
            }

            base.Dispose(disposing);
        }

        protected virtual void TryCommit()
        {
            try
            {
                Transaction.Commit();
            }
            catch (Exception)
            {
                Transaction.Rollback();
                throw;
            }
        }
    }
}
