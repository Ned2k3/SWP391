using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using ProjectSWP391.Models;

namespace ProjectSWP391.DAO
{
    public class ServiceManagementDAO
    {
        public List<Service> GetServices(string search, bool isSearch, bool isAscendingPrice)
        {
            var context = new SWP391_V4Context();

            var services = context.Services.Include(s => s.Scategory).Select(ser => new Service
            {
                ServiceId = ser.ServiceId,
                ServiceName = ser.ServiceName,
                Description = ser.Description ?? string.Empty,
                Price = ser.Price,
                Image = ser.Image ?? string.Empty,
                IsActive = ser.IsActive,
                ScategoryId = ser.ScategoryId,
                Scategory = ser.Scategory
            }).AsQueryable();
            try
            {
                if (!string.IsNullOrEmpty(search))
                {
                    services = services.Where(s => s.ServiceName.Contains(search) || s.Price.ToString().Contains(search));
                }
                else if (isSearch == true && string.IsNullOrEmpty(search))
                {
                    services = services.Where(s => false);
                }
                else
                {
                    services = services;

                }

                if (isAscendingPrice)
                {
                    services = services.OrderBy(s => s.Price);
                }
                else
                {
                    services = services.OrderByDescending(s => s.Price);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return services.ToList();
        }

        public List<ServiceCategory> GetServiceCategories()
        {
            var list = new List<ServiceCategory>();
            try
            {
                using (var context = new SWP391_V4Context())
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
                using (var context = new SWP391_V4Context())
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
                using (var context = new SWP391_V4Context())
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
                using (var context = new SWP391_V4Context())
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
                using (var context = new SWP391_V4Context())
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
