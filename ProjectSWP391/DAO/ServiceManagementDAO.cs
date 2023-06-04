using Microsoft.EntityFrameworkCore;
using ProjectSWP391.Models;

namespace ProjectSWP391.DAO
{
    public class ServiceManagementDAO
    {
        public List<Service> GetServices()
        {
            var list = new List<Service>();
            try
            {
                using (var context = new SWP391Context())
                {
                    list = context.Services.Include(s => s.Scategory).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return list;
        }

        public List<ServiceCategory> GetServiceCategories()
        {
            var list = new List<ServiceCategory>();
            try
            {
                using (var context = new SWP391Context())
                {
                    list = context.ServiceCategories.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return list;
        }

        public Service GetServiceById(int id)
        {
            Service service = new Service();
            try
            {
                using (var context = new SWP391Context())
                {
                    service = context.Services.SingleOrDefault(s => s.ServiceId == id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return service;
        }

        public void AddService(Service service)
        {
            try
            {
                using (var context = new SWP391Context())
                {
                    context.Services.Add(service);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void EditService(Service service)
        {
            try
            {
                using (var context = new SWP391Context())
                {
                    context.Entry<Service>(service).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void DeleteService(Service service)
        {
            try
            {
                using (var context = new SWP391Context())
                {
                    context.Services.Remove(service);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
