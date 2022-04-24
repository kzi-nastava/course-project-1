using HealthCare.Data.Context;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Services;
using HealthCare.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
//builder.Services.AddControllers().AddJsonOptions(x =>
//   x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);


//Repositories
builder.Services.AddTransient<IAntiTrollRepository, AntiTrollRepository>();
builder.Services.AddTransient<IAnamnesisRepository, AnamnesisRepository>();
builder.Services.AddTransient<ICredentialsRepository, CredentialsRepository>();
builder.Services.AddTransient<IDoctorRepository, DoctorRepository>();
builder.Services.AddTransient<IEquipmentRepository, EquipmentRepository>();
builder.Services.AddTransient<IEquipmentTypeRepository, EquipmentTypeRepository>();
builder.Services.AddTransient<IExaminationRepository, ExaminationRepository>();
builder.Services.AddTransient<IExaminationApprovalRepository, ExaminationApprovalRepository>();
builder.Services.AddTransient<IInventoryRepository, InventioryRepository>();
builder.Services.AddTransient<IManagerRepository, ManagerRepository>();
builder.Services.AddTransient<IMedicalRecordRepository, MedicalRecordRepository>();
builder.Services.AddTransient<IOperationRepository, OperationRepository>();
builder.Services.AddTransient<IPatientRepository, PatientRepository>();
builder.Services.AddTransient<IRoomRepository, RoomRepository>();
builder.Services.AddTransient<IRoomTypeRepository, RoomTypeRepository>();
builder.Services.AddTransient<ISecretaryRepository, SecretaryRepository>();
builder.Services.AddTransient<ITransferRepository, TransferRepository>();
builder.Services.AddTransient<IUserRoleRepository, UserRoleRepository>();

//Domain
builder.Services.AddTransient<IAntiTrollService, AntiTrollService>();
builder.Services.AddTransient<IAnamnesisService, AnamnesisService>();
builder.Services.AddTransient<ICredentialsService, CredentialsService>();
builder.Services.AddTransient<IDoctorService, DoctorService>();
builder.Services.AddTransient<IEquipmentService, EquipmentService>();
builder.Services.AddTransient<IEquipmentTypeService, EquipmentTypeService>();
builder.Services.AddTransient<IExaminationApprovalService, ExaminationApprovalService>();
builder.Services.AddTransient<IExaminationService, ExaminationService>();
builder.Services.AddTransient<IInventoryService, InventoryService>();
builder.Services.AddTransient<IManagerService, ManagerService>();
builder.Services.AddTransient<IMedicalRecordService, MedicalRecordService>();
builder.Services.AddTransient<IOperationService, OperationService>();
builder.Services.AddTransient<IPatientService, PatientService>();
builder.Services.AddTransient<IRoomService, RoomService>();
builder.Services.AddTransient<IRoomTypeService, RoomTypeService>();
builder.Services.AddTransient<ISecretaryService, SecretaryService>();
builder.Services.AddTransient<ITransferService, TransferService>();
builder.Services.AddTransient<IUserRoleService, UserRoleService>();


var connectionString = builder.Configuration.GetConnectionString("HealthCareConnection");
builder.Services.AddDbContext<HealthCareContext>(x => x.UseSqlServer(connectionString));
//builder.Services.AddDbContext<HealthCareContext>(x => x.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
builder.Services.AddDbContext<HealthCareContext>(x => x.EnableSensitiveDataLogging());


builder.Services.AddCors(options => {
    options.AddPolicy("CorsPolicy", 
        corsBuilder => corsBuilder.WithOrigins("http://localhost:43022").AllowAnyMethod()
           .AllowAnyHeader()
            .AllowCredentials());
});


var app = builder.Build();

// Configure the HTTP request pipeline.`
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    
}
else {
    app.UseSwagger();
    //app.UseSwaggerUI();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints => endpoints.MapControllers());

//app.MapRazorPages();

app.Run();
