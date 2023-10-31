using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TimeTable.Repository;
using TimeTable.Respository.Interfaces;

namespace TimeTable.Respository.Configs
{
    public static class InjectionRepositoryExtension
    {
        public static void DependencyInjectionRepository(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IClassRepons, ClassRepons>();
            services.AddScoped<IClassRoomRepons, ClassRoomRepons>();
            services.AddScoped<IChangerPasswordrepons, ChangerPasswordRepons>();
            services.AddScoped<IEditAccountRepons, EditAccountRepons>();
            services.AddScoped<ILectureSchedureRepons, LectureSchedureRepons>();
            services.AddScoped<ILecture_ScheduleManagerRepons, Lecture_ScheduleManagerRepons>();
            services.AddScoped<ISubjectRepons, SubjectRepons>();
            services.AddScoped<IUserManagerRepons, UserManagerRepons>();
            services.AddScoped<IUserRepository, UserRepository>();
        }
    }
}
