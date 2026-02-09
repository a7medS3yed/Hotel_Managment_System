
using HMS.Api.Extensions;
using HMS.Core.Contracts;
using HMS.Core.Entities.SecurityModul;
using HMS.InfraStructure.Data.Context;
using HMS.InfraStructure.Data.DataSeed;
using HMS.InfraStructure.ExternalService;
using HMS.InfraStructure.Repositories;
using HMS.Service.Helper;
using HMS.Service.MappingProfile;
using HMS.Service.Services;
using HMS.ServiceAbstraction;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<HMSDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IRoomService, RoomService>();
            builder.Services.AddTransient<IAttachmentService, AttachmentService>();
            builder.Services.AddAutoMapper(typeof(ServiceAssemblyReference).Assembly);

            builder.Services.AddIdentityCore<HotelUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<HMSDbContext>();

            builder.Services.AddScoped<IDataInitializer, DataInitializer>();
            builder.Services.AddScoped<IAuthService, AuthService>();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.SaveToken = true;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = builder.Configuration["Jwt:Issuer"],
                            ValidAudience = builder.Configuration["Jwt:Audience"],
                            IssuerSigningKey = new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
                            )
                        };
                    });
            builder.Services.Configure<EmailSettings>(
                builder.Configuration.GetSection("EmailSettings"));

            builder.Services.AddTransient<IEmailService, EmailService>();
            builder.Services.AddScoped<IBookingService, BookingService>();
            builder.Services.AddHttpClient<IPaymentService, IPaymentService>();
            //builder.Services.AddHttpClient<IAiModerationService, OpenAiModerationService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<IFeedbackService, FeedbackService>();
            builder.Services.AddHttpClient<IAiModerationService, HuggingFaceModerationService>();





            var app = builder.Build();

            await app.MigrateDatabaseAsync();
            await app.SeedingIdentityDataAsync();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();

            //app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
