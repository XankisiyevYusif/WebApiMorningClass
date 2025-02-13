using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebApiMorning.Data;
using WebApiMorning.Entities;
using WebApiMorning.Repositories.Abstract;

namespace WebApiMorning.Repositories.Concrete
{
    public class StudentRepository : IStudentRepository
    {
        private readonly StudentDbContext _context;

        public StudentRepository(StudentDbContext context)
        {
            _context = context;
        }

        public async Task<Student> Add(Student entity)
        {
            await _context.Students.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> Delete(Student entity)
        {
            await Task.Run(() =>
            {
                _context.Students.Remove(entity);
            });
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Student> Get(Expression<Func<Student, bool>> predicate)
        {
            var student = await _context.Students.FirstOrDefaultAsync(predicate);
            return student;
        }

        public async Task<IEnumerable<Student>> GetAll()
        {
            return await _context.Students.ToListAsync();
        }

        public async Task<Student> Update(Student entity)
        {
            await Task.Run(() =>
            {
                _context.Students.Update(entity);
            });
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
