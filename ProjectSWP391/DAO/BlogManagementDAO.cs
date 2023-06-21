using Microsoft.EntityFrameworkCore;
using ProjectSWP391.Models;

namespace ProjectSWP391.DAO
{
    public class BlogManagementDAO
    {
        public List<Blog> GetBlogs(string search, bool isSearch, bool isAscending)
        {
            var context = new SWP391_V4Context();

            var blogs = context.Blogs.Include(b => b.Account).Select(b => new Blog
            {
                BlogId = b.BlogId,
                Title = b.Title,
                Content = b.Content,
                BlogDate = b.BlogDate,
                AccountId = b.AccountId,
                Account = b.Account
            }).AsQueryable();
            try
            {
                if (!string.IsNullOrEmpty(search))
                {
                    blogs = blogs.Where(b => b.Title.Contains(search) || b.Content.Contains(search));
                }
                else if (isSearch == true && string.IsNullOrEmpty(search))
                {
                    blogs = blogs.Where(b => false);
                }
                else
                {
                    blogs = blogs;
                }

                if (isAscending)
                {
                    blogs = blogs.OrderBy(b => b.BlogDate);
                }
                else
                {
                    blogs = blogs.OrderByDescending(b => b.BlogDate);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return blogs.ToList();
        }

        public List<Account> GetAccounts()
        {
            var list = new List<Account>();
            try
            {
                using (var context = new SWP391_V4Context())
                {
                    list = context.Accounts.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return list;
        }

        public Blog GetBlogById(int id)
        {
            Blog blog = new Blog();
            try
            {
                using (var context = new SWP391_V4Context())
                {
                    blog = context.Blogs.SingleOrDefault(b => b.BlogId == id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return blog;
        }

        public void AddBlog(Blog blog)
        {
            try
            {
                using (var context = new SWP391_V4Context())
                {
                    context.Blogs.Add(blog);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void EditBlog(Blog blog)
        {
            try
            {
                using (var context = new SWP391_V4Context())
                {
                    context.Entry<Blog>(blog).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void DeleteBlog(Blog blog)
        {
            try
            {
                using (var context = new SWP391_V4Context())
                {
                    context.Blogs.Remove(blog);
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
