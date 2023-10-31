using Dapper;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using TimeTable.DataContext.Data;
using TimeTable.DataContext.Models;
using TimeTable.Respository.Interfaces;

namespace TimeTable.Repository
{
    public class Lecture_ScheduleManagerRepons : ILecture_ScheduleManagerRepons
    {
        private readonly ConnectToSql _connectToSql;

        public Lecture_ScheduleManagerRepons(ConnectToSql connectToSql) 
        {
            _connectToSql = connectToSql;
        }
        public Task<string> AddLecture_ScheduleManagerAsync(Lecture_ScheduleManagerModel model)
        {
            throw new NotImplementedException();
        }

        public Task<string> DeleteLecture_ScheduleManagerAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<(List<Lecture_ScheduleManagerModel>, int)> GetAllLecture_ScheduleManagerAsync(int pageIndex, int pageSize)
        {
            try
            {
                using (var connect = _connectToSql.CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@pageIndex", pageIndex);
                    parameters.Add("@pageSize", pageSize);
                    parameters.Add("@totalRecords", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    var result = await connect.QueryAsync<Lecture_ScheduleManagerModel>(
                        "GetAllTimeTable",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    int totalRecords = parameters.Get<int>("@totalRecords");
                    return (result.ToList(), totalRecords);
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<(List<Lecture_ScheduleManagerModel>, int)> GetLecture_ScheduleManagerByNameAsync(string name, int pageIndex, int pageSize)
        {
            try
            {
                using (var connect = _connectToSql.CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@Name", name);
                    parameters.Add("@pageIndex", pageIndex);
                    parameters.Add("@pageSize", pageSize);
                    parameters.Add("@totalRecords", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    var result = await connect.QueryAsync<Lecture_ScheduleManagerModel>(
                        "GetAllLecture_ScheduleManagerByName",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    int totalRecords = parameters.Get<int>("@totalRecords");
                    return (result.ToList(), totalRecords);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<LectureSchedureMapUserModel>> SchedulingAscync(SchedulingInputModel schedulingInputModel)
        {
            try
            {
                using (var connect = _connectToSql.CreateConnection())
                {

                    int daysDifference = (int)(schedulingInputModel.DateEnd - schedulingInputModel.DateStart).TotalDays;
                    List<Class> classList = new List<Class> { };
                    List<ClassRooms> ClassRoomList = new List<ClassRooms> { };
                    List<Subject> SubjectList = new List<Subject> { };
                    foreach (var idclass in schedulingInputModel.Idclasses)
                    {
                        var classes = await connect.QueryFirstOrDefaultAsync<Class>("GetById", new { NameTable = "Class", Id = idclass }, commandType: CommandType.StoredProcedure);
                        classList.Add(classes);
                    }
                    foreach (var idclassroom in schedulingInputModel.IdclassRooms)
                    {
                        var classroom = await connect.QueryFirstOrDefaultAsync<ClassRooms>("GetById", new { NameTable = "ClassRooms", Id = idclassroom }, commandType: CommandType.StoredProcedure);
                        ClassRoomList.Add(classroom);
                    }
                    int totalAprear = 0;
                    foreach (var idsubject in schedulingInputModel.Idsubjects)
                    {
                        var subject = await connect.QueryFirstOrDefaultAsync<Subject>("GetById", new { NameTable = "Subjects", Id = idsubject }, commandType: CommandType.StoredProcedure);
                        subject.appear = (subject.Credits * 5) / (daysDifference / 7);
                        SubjectList.Add(subject);
                        totalAprear += subject.appear;

                    }
                    List<int[,]> timeTableForTotalSub = new List<int[,]>();
                    List<string[,]> timeTableForTotalClas = new List<string[,]>();
                    bool flag1 = false;
                    bool flag2 = false;
                    bool flag3 = false;
                    bool flag4 = false;
                    List<Guid> listClassRoom = schedulingInputModel.IdclassRooms;
                    List<bool[,]> listClassRoomCheck = new List<bool[,]>();
 
                    int rows = 4;
                    int cols = 6;
                    for(int index = 0; index< listClassRoom.Count; index++)
                    {
                        bool[,] classRoom = new bool[rows,cols];
                        for(int i = 0; i< rows; i++)
                        {
                            for(int j = 0; j<cols; j++)
                            {
                                classRoom[i,j] = true;
                            }
                        }
                        listClassRoomCheck.Add(classRoom);
                    }
                    softTimeTable(SubjectList, timeTableForTotalSub, timeTableForTotalClas, classList, flag1, flag2, flag3, flag4, schedulingInputModel,schedulingInputModel.DateStart, schedulingInputModel.DateEnd, listClassRoom, listClassRoomCheck,totalAprear);
                }

                return new List<LectureSchedureMapUserModel>();
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }
        }
        public int IsClassMoreThanClasses(int[,] result, int classes, int i, int j)
        {
            if (result[i, j] > classes)
            {
                result[i, j] = 0;
            }
            return result[i, j];
        }
        public int[,] softFirst1( int classes)
        {
            int rows = 4;
            int cols = 6;
            int[,] result = new int[rows, cols];
            int[,] values = {
                { 1, 2, 24, 4, 5, 6 },
                { 7, 8, 9, 10, 11, 12 },
                { 13, 3, 14, 15, 16, 17 },
                { 18, 19, 20, 21, 22, 23}
            };
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    result[i, j] = values[i, j];
                    IsClassMoreThanClasses(result, classes, i, j);
                }
            }
            return result;
        }
        public int[,] softFirst1Update(int classes, int totalCreadit, int appear)
        {
            int rows = 4;
            int cols = 6;
            int[,] result = new int[rows, cols];
            if (totalCreadit == 10)
            {
                int[,] values = {
                    { 1, 13, 6, 18, 11, 23 },
                    { 4, 16, 9, 21, 2, 14 },
                    { 7, 19, 12, 24, 5, 17 },
                    { 10, 22, 3, 15, 8, 20 }
                };
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        result[i, j] = values[i, j];
                        IsClassMoreThanClasses(result, classes, i, j);
                    }
                }
            }
            else if (totalCreadit == 8 )
            {
                int[,] values = {
                    { 10, 1, 4, 7, 22, 13 },
                    { 16, 19, 11, 2, 5, 8 },
                    { 14, 23, 20, 17, 12, 3 },
                    { 6, 9, 15, 24, 21, 18 }
                };
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        result[i, j] = values[i, j];
                        IsClassMoreThanClasses(result, classes, i, j);
                    }
                }
            }
            return result;
        }
        public int[,] softFirst2(int classes)
        {
            int rows = 4;
            int cols = 6;
            int clas = 1;
            int[,] result = new int[rows, cols];
            int[,] predefinedValues ={
                { 10, 20, 18, 16, 14, 21 },
                { 5, 9, 17, 13, 1, 22 },
                { 19, 4, 8, 2, 6, 11 },
                { 24, 23, 3, 7, 12, 15 }
            };
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    result[i, j] = predefinedValues[i, j];
                    IsClassMoreThanClasses(result, classes, i, j);
                }
            }
            return result;
        }
        public int[,] softFirst2Update(int classes, int totalCreadit, int appear)
        {
            int rows = 4;
            int cols = 6;
            int[,] result = new int[rows, cols];
            if (totalCreadit == 10)
            {
                int[,] values = {
                    { 3, 15, 8, 20, 1, 13 },
                    { 6, 18, 11, 23, 4, 16 },
                    { 9, 21, 2, 14, 7, 19 },
                    { 12, 24, 5, 17, 10, 22 }
                };
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        result[i, j] = values[i, j];
                        IsClassMoreThanClasses(result, classes, i, j);
                    }
                }
            }
            else if (totalCreadit == 8 )
            {
                int[,] values = {
                    { 13, 22, 19, 16, 1, 10 },
                    { 7, 4, 23, 14, 17, 20 },
                    { 2, 11, 8, 5, 24, 15 },
                    { 18, 21, 3, 12, 9, 6 }
                };
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        result[i, j] = values[i, j];
                        IsClassMoreThanClasses(result, classes, i, j);
                    }
                }
            }
            return result;
        }
        public int[,] softFirst3(int classes, bool flag)
        {
            int rows = 4;
            int cols = 6;
            int[,] result = new int[rows, cols];
            int[,] values = {
                { 16, 17, 1, 21, 15, 11},
                { 4, 18, 19, 20, 12, 3 },
                { 10,14, 22, 13, 8, 24 },
                { 5, 9, 23, 2, 6, 7}
            };
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    result[i, j] = values[i, j];
                    IsClassMoreThanClasses(result, classes, i, j);
                }
            }
            flag = true;
            return result;
        }
        public int[,] softFirst3Update(int classes, int totalCreadit, string[,] timeTableForEachClas,string subjectId)
        {
            int rows = 4;
            int cols = 6;
            int[,] result = new int[rows, cols];
            if (totalCreadit == 10)
            {
                int[,] values = {
                    { 5, 17, 10, 22, 3, 15 },
                    { 8, 20, 1, 13, 6, 18 },
                    { 11, 23, 4, 16, 9, 21 },
                    { 2, 14, 7, 19, 12, 24 }
                };
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        result[i, j] = values[i, j];
                        IsClassMoreThanClasses(result, classes, i, j);
                    }
                }
            }
            else if (totalCreadit == 8)
            {
                FindAndUpdate(timeTableForEachClas, subjectId);
            }
            return result;
        }
        public int[,] softSecond1( int classes)
        {
            int rows = 4;
            int cols = 6;
            int[,] result = new int[rows, cols];
            int[,] values = {
                { 7, 8, 9, 10, 11, 12},
                { 1, 2, 24, 4, 5, 6 },
                { 18,19, 20, 21, 22, 23 },
                { 13, 3, 14, 15, 16, 17}
            };
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    result[i, j] = values[i, j];
                    IsClassMoreThanClasses(result, classes, i, j);
                }
            }
            return result;
        }
        public int[,] softSecond1Update(int classes, int totalCreadit, int appear)
        {
            int rows = 4;
            int cols = 6;
            int[,] result = new int[rows, cols];
            if (totalCreadit == 10)
            {
                int[,] values = {
                    { 13, 1, 18, 6, 23, 11 },
                    { 16, 4, 21, 9, 14, 2 },
                    { 19, 7, 24, 12, 17, 5 },
                    { 22, 10, 15, 3, 20, 8 }
                };
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        result[i, j] = values[i, j];
                        IsClassMoreThanClasses(result, classes, i, j);
                    }
                }
            }
            else if (totalCreadit == 8)
            {
                int[,] values = {
                    { 7, 4, 1, 10, 19, 16},
                    { 13, 22, 8, 5, 2, 11 },
                    { 17, 20, 23, 14, 9, 6 },
                    { 3, 12, 18, 21, 24, 15 }
                };
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        result[i, j] = values[i, j];
                        IsClassMoreThanClasses(result, classes, i, j);
                    }
                }
            }
            return result;
        }
        public int[,] softSecond2( int classes)
        {
            int rows = 4;
            int cols = 6;
            int[,] result = new int[rows, cols];
            int[,] values = {
                { 21, 18, 17, 13, 1, 20},
                { 10, 14, 16, 2, 6, 11 },
                { 5,9, 3, 7, 12, 15 },
                { 19, 4, 8, 22, 23, 24}
            };
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    result[i, j] = values[i, j];
                    IsClassMoreThanClasses(result, classes, i, j);
                }
            }
            return result;
        }
        public int[,] softSecond2Update(int classes, int totalCreadit, int appear)
        {
            int rows = 4;
            int cols = 6;
            int[,] result = new int[rows, cols];
            if (totalCreadit == 10)
            {
                int[,] values = {
                    { 15, 3, 20, 8, 13, 1 },
                    { 18, 6, 23, 11, 16, 4 },
                    { 21, 9, 14, 2, 19, 7 },
                    { 24, 12, 17, 5, 22, 10 }
                };
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        result[i, j] = values[i, j];
                        IsClassMoreThanClasses(result, classes, i, j);
                    }
                }
            }
            else if (totalCreadit == 8 )
            {
                int[,] values = {
                    { 16, 19, 22, 13, 4, 7 },
                    { 10, 1, 20, 17, 14, 23},
                    { 5, 8, 11, 2, 21, 18 },
                    { 15, 24, 6, 9, 12, 3 }
                };
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        result[i, j] = values[i, j];
                        IsClassMoreThanClasses(result, classes, i, j);
                    }
                }
            }
            return result;
        }
        public int[,] softSecond3( int classes, bool flag)
        {
            int rows = 4;
            int cols = 6;
            int[,] result = new int[rows, cols];
            int[,] values = {
                { 11, 15, 21, 1, 17, 16},
                { 3, 12, 20, 19, 18, 4 },
                { 24,8, 13, 22, 14, 10 },
                { 7, 6, 2, 23, 9, 5}
            };
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    result[i, j] = values[i, j];
                    IsClassMoreThanClasses(result, classes, i, j);
                }
            }
            flag = true;
            return result;
        }
        public int[,] softSecond3Update(int classes, int totalCreadit, string[,] timeTableForEachClas, string subjectId)
        {
            int rows = 4;
            int cols = 6;
            int[,] result = new int[rows, cols];

            if (totalCreadit == 10)
            {
                int[,] values = {
                    { 17, 5, 22, 10, 15, 3 },
                    { 20, 8, 13, 1, 18, 6 },
                    { 23, 11, 16, 4, 21, 9 },
                    { 14, 2, 19, 7, 24, 12 }
                };
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        result[i, j] = values[i, j];
                        IsClassMoreThanClasses(result, classes, i, j);
                    }
                }
            }
            else if (totalCreadit == 8)
            {
                FindAndUpdate(timeTableForEachClas, subjectId);
            }
            return result;
        }
        public int[,] softThird1(int classes)
        {
            int rows = 4;
            int cols = 6;
            int[,] result = new int[rows, cols];
            int[,] values = {
                { 6, 5, 4, 24, 2, 1},
                { 12, 11, 10, 9, 8, 7 },
                { 17,16, 15, 14, 3, 13 },
                { 23, 22, 21, 20, 19, 18}
            };
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    result[i, j] = values[i, j];
                    IsClassMoreThanClasses(result, classes, i, j);
                }
            }
            return result;
        }
        public int[,] softThird1Update(int classes, int totalCreadit, int appear)
        {
            int rows = 4;
            int cols = 6;
            int[,] result = new int[rows, cols];
            if (totalCreadit == 10)
            {
                int[,] values = {
                    { 8, 20, 1, 13, 6, 18 },
                    { 11, 23, 4, 16, 9, 21 },
                    { 2, 14, 7, 19, 12, 24 },
                    { 5, 17, 10, 22, 3, 15 }
                };
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        result[i, j] = values[i, j];
                        IsClassMoreThanClasses(result, classes, i, j);
                    }
                }
            }
            else if (totalCreadit == 8 )
            {
                int[,] values = {
                    { 4, 7, 10, 1, 16, 19 },
                    { 22, 13, 5, 8, 11, 2 },
                    { 20, 17, 14, 23, 6, 9 },
                    { 12, 3, 21, 18, 15, 24 }
                };
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        result[i, j] = values[i, j];
                        IsClassMoreThanClasses(result, classes, i, j);
                    }
                }
            }
            return result;
        }
        public int[,] softThird2( int classes)
        {
            int rows = 4;
            int cols = 6;
            int[,] result = new int[rows, cols];
            int[,] values = {
                { 23, 14, 16, 18, 20, 10},
                { 22, 1, 13, 17, 9, 5 },
                { 11,6, 2, 8, 4, 19 },
                { 15, 12, 7, 3, 24, 21}
            };
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    result[i, j] = values[i, j];
                    IsClassMoreThanClasses(result, classes, i, j);
                }
            }
            return result;
        }
        public int[,] softThird2Update(int classes, int totalCreadit, int appear)
        {
            int rows = 4;
            int cols = 6;
            int[,] result = new int[rows, cols];
            if (totalCreadit == 10)
            {
                int[,] values = {
                    { 10, 22, 3, 15, 8, 20 },
                    { 1, 13, 6, 18, 11, 23 },
                    { 4, 16, 9, 21, 2, 14 },
                    { 7, 19, 12, 24, 5, 17 }
                };
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        result[i, j] = values[i, j];
                        IsClassMoreThanClasses(result, classes, i, j);
                    }
                }
            }
            else if (totalCreadit == 8 )
            {
                int[,] values = {
                    { 19, 16, 13, 22, 7, 4 },
                    { 1, 10, 17, 20, 23, 14 },
                    { 8, 5, 2, 11, 18, 21 },
                    { 24, 15, 9, 6, 3, 12 }
                };
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        result[i, j] = values[i, j];
                        IsClassMoreThanClasses(result, classes, i, j);
                    }
                }
            }
            return result;
        }
        public int[,] softThird3( int classes, bool flag)
        {
            int rows = 4;
            int cols = 6;
            int[,] result = new int[rows, cols];
            int[,] values = {
                { 9, 21, 11, 12, 22, 4},
                { 2, 7, 5, 6, 23, 14 },
                { 8,13, 18, 1, 10, 3 },
                { 20, 15, 24, 19, 17, 16}
            };
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    result[i, j] = values[i, j];
                    IsClassMoreThanClasses(result, classes, i, j);
                }
            }
            flag = true;
            return result;
        }
        public int[,] softThird3Update(int classes, int totalCreadit, string[,] timeTableForEachClas, string subjectId)
        {
            int rows = 4;
            int cols = 6;
            int[,] result = new int[rows, cols];
            if (totalCreadit == 10)
            {
                int[,] values = {
                    { 5, 17, 10, 22, 3, 15 },
                    { 8, 20, 1, 13, 6, 18 },
                    { 11, 23, 4, 16, 9, 21 },
                    { 2, 14, 7, 19, 12, 24 }
                };
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        result[i, j] = values[i, j];
                        IsClassMoreThanClasses(result, classes, i, j);
                    }
                }
            }

            else if (totalCreadit == 8)
            {
                FindAndUpdate(timeTableForEachClas, subjectId);
            }
            return result;
        }
        public int[,] softFour1( int classes)
        {
            int rows = 4;
            int cols = 6;
            int[,] result = new int[rows, cols];
            int[,] values = {
                { 12, 11, 10, 9, 8, 7},
                { 6, 5, 4, 24, 2, 1 },
                { 23,22, 21, 20, 19, 18 },
                { 17, 16, 15, 14, 3, 13}
            };
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    result[i, j] = values[i, j];
                    IsClassMoreThanClasses(result, classes, i, j);
                }
            }
            return result;
        }
        public int[,] softFour1Update(int classes, int totalCreadit, int appear)
        {
            int rows = 4;
            int cols = 6;
            int[,] result = new int[rows, cols];
            if (totalCreadit == 10)
            {
                int[,] values = {
                    { 20, 8, 13, 1, 18, 6 },
                    { 23, 11, 16, 4, 21, 9 },
                    { 14, 2, 19, 7, 24, 12 },
                    { 17, 5, 22, 10, 15, 3 }
                };
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        result[i, j] = values[i, j];
                        IsClassMoreThanClasses(result, classes, i, j);
                    }
                }
            }
            else if (totalCreadit == 8)
            {
                int[,] values = {
                    { 1, 10, 7, 4, 13, 22 },
                    { 19, 16, 2, 11, 8, 5 },
                    { 23, 14, 17, 20, 3, 12 },
                    { 9, 6, 24, 15, 18, 21 }
                };
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        result[i, j] = values[i, j];
                        IsClassMoreThanClasses(result, classes, i, j);
                    }
                }
            }
            return result;
        }
        public int[,] softFour2( int classes)
        {
            int rows = 4;
            int cols = 6;
            int[,] result = new int[rows, cols];
            int[,] values = {
                { 20, 1, 13, 17, 18, 23},
                { 11, 6, 2, 16, 14, 10 },
                { 15,12, 7, 3, 9, 5 },
                { 21, 24, 22, 8, 4, 19}
            };
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    result[i, j] = values[i, j];
                    IsClassMoreThanClasses(result, classes, i, j);
                }
            }
            return result;
        }
        public int[,] softFour2Update(int classes, int totalCreadit,int appear)
        {
            int rows = 4;
            int cols = 6;
            int[,] result = new int[rows, cols];
            if (totalCreadit == 10)
            {
                int[,] values = {
                    { 22, 10, 15, 3, 20, 8 },
                    { 13, 1, 18, 6, 23, 11 },
                    { 16, 4, 21, 9, 14, 2 },
                    { 19, 7, 24, 12, 17, 5 }
                };
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        result[i, j] = values[i, j];
                        IsClassMoreThanClasses(result, classes, i, j);
                    }
                }
            }
            else if (totalCreadit == 8)
            {
                int[,] values = {
                    { 22, 13, 16, 19, 10, 1 },
                    { 4, 7, 14, 23, 20, 17 },
                    { 11, 2, 5, 8, 15, 24 },
                    { 21, 18, 12, 3, 6, 9 }
                };
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        result[i, j] = values[i, j];
                        IsClassMoreThanClasses(result, classes, i, j);
                    }
                }
            }
            return result;
        }
        public int[,] softFour3( int classes, bool flag)
        {
            int rows = 4;
            int cols = 6;
            int[,] result = new int[rows, cols];
            int[,] values = {
                { 4, 22, 12, 11, 21, 9},
                { 14, 23, 6, 5, 7, 2 },
                { 3,10, 1, 18, 13, 8 },
                { 16, 17, 19, 24, 15, 20}
            };
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    result[i, j] = values[i, j];
                    IsClassMoreThanClasses(result, classes, i, j);
                }
            }
            flag = true;
            return result;
        }
        public int[,] softFour3Update(int classes, int totalCreadit, string[,] timeTableForEachClas, string subjectId)
        {
            int rows = 4;
            int cols = 6;
            int[,] result = new int[rows, cols];
            if (totalCreadit == 10)
            {
                int[,] values = {
                    { 17, 5, 22, 10, 15, 3 },
                    { 20, 8, 13, 1, 18, 6 },
                    { 23, 11, 16, 4, 21, 9 },
                    { 14, 2, 19, 7, 24, 12 }
                };
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        result[i, j] = values[i, j];
                        IsClassMoreThanClasses(result, classes, i, j);
                    }
                }
            }
            else if (totalCreadit == 8)
            {
                FindAndUpdate(timeTableForEachClas, subjectId);
            }
            return result;
        }
        public void fillSubForEachClass(int[,] timeTableForEchSub, string[,] timeTableForEachClas, Guid subID, int clas)
        {
            int rows = 4;
            int cols = 6;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (timeTableForEchSub[i, j] == clas)
                    {
                        timeTableForEachClas[i, j] = subID.ToString();
                    }
                }
            }
        }
        public void FindAndUpdate(string[,] timeTableForEachClas, string newValue)
        {
            int rows = 4;
            int cols = 6;
            bool check = false; ;
            bool breakOut = false;
            // Tìm điểm bắt đầu không rỗng
            for (int i = 0; i < rows; i++)
            {
                if (breakOut)
                {
                    return;
                }
                for (int j = 0; j < cols; j++)
                {
                    // Nếu thấy thì gắn cờ check = true
                    if (!string.IsNullOrEmpty(timeTableForEachClas[i, j]))
                    {
                        check = true;
                    }
                    // Bắt đầu tìm điểm không rỗng
                    if (string.IsNullOrEmpty(timeTableForEachClas[i, j]))
                    {
                        if (check)
                        {
                            timeTableForEachClas[i, j] = newValue;
                            breakOut = true;
                            break;
                        }
                    }
                }
            }
        }

        List<Guid> listIdSchedule = new List<Guid>();
        public async Task softTimeTable(List<Subject> listSubject, List<int[,]> timeTableForTotalSub, List<string[,]> timeTableForTotalClas, List<Class> classList, bool flag1, bool flag2, bool flag3, bool flag4, SchedulingInputModel schedulingInputModel, DateTime startDate, DateTime endDate, List<Guid> listClassRoom, List<bool[,]> listClassRoomCheck, int totalAppear)
        {
            int rows = 4;
            int cols = 6;
            int[,] TimeTbForEarchSub = new int[rows, cols];
            int totalClass = classList.Count;
            int cls = 0;


            if (schedulingInputModel.IdclassRooms.Count < classList.Count)
            {
                // Vì ko có cách xếp cho tần suất 9 và 10
                if (totalAppear == 8 && listSubject.Count <= 4)
                {
                    foreach (var idclasses in schedulingInputModel.Idclasses)
                    {
                        Guid idSchedule = Guid.NewGuid();
                        listIdSchedule.Add(idSchedule);
                        using (var connect = _connectToSql.CreateConnection())
                        {
                            SqlCommand cmd = new SqlCommand();
                            cmd.CommandType = CommandType.Text;
                            cmd.CommandText = "INSERT INTO Lecture_Schedules (idLecture_Schedule,idClass, createDate) VALUES (@id,@class, @createDate)";
                            cmd.Parameters.AddWithValue("@id", idSchedule);
                            cmd.Parameters.AddWithValue("@class", idclasses);
                            cmd.Parameters.AddWithValue("@createDate", DateTime.Now);
                            cmd.Connection = (SqlConnection)connect;
                            connect.Open();
                            int kq = await cmd.ExecuteNonQueryAsync();
                            int test = kq;
                        }
                        int i = 0;
                        string[,] timeTableForEachClas = new string[rows, cols];
                        foreach (var idsubject in listSubject)
                        {
                            if (i == 0)
                            {
                                if (listSubject[i].appear == 1)
                                {
                                    TimeTbForEarchSub = softFirst1Update(totalClass, totalAppear, listSubject[i].appear);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                }
                                else if (listSubject[i].appear == 2)
                                {
                                    TimeTbForEarchSub = softFirst1Update(totalClass, totalAppear, listSubject[i].appear);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softFirst2Update(totalClass, totalAppear, listSubject[i].appear);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);

                                }
                                else if (listSubject[i].appear == 3)
                                {
                                    TimeTbForEarchSub = softFirst1Update(totalClass, totalAppear, listSubject[i].appear);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softFirst2Update(totalClass, totalAppear, listSubject[i].appear);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    softFirst3Update(totalClass, totalAppear, timeTableForEachClas, listSubject[i].Id.ToString());
                                }

                            }
                            else if (i == 1)
                            {
                                if (listSubject[i].appear == 1)
                                {
                                    TimeTbForEarchSub = softSecond1Update(totalClass, totalAppear, listSubject[i].appear);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                }
                                else if (listSubject[i].appear == 2)
                                {
                                    TimeTbForEarchSub = softSecond1Update(totalClass, totalAppear, listSubject[i].appear);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softSecond2Update(totalClass, totalAppear, listSubject[i].appear);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                }
                                else if (listSubject[i].appear == 3)
                                {
                                    TimeTbForEarchSub = softSecond1Update(totalClass, totalAppear, listSubject[i].appear);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softSecond2Update(totalClass, totalAppear, listSubject[i].appear);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softSecond3Update(totalClass, totalAppear, timeTableForEachClas, listSubject[i].Id.ToString());
                                }
                            }
                            else if (i == 2)
                            {
                                if (listSubject[i].appear == 1)
                                {
                                    TimeTbForEarchSub = softThird1Update(totalClass, totalAppear, listSubject[i].appear);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                }
                                else if (listSubject[i].appear == 2)
                                {
                                    TimeTbForEarchSub = softThird1Update(totalClass, totalAppear, listSubject[i].appear);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softThird2Update(totalClass, totalAppear, listSubject[i].appear);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                }
                                else if (listSubject[i].appear == 3)
                                {
                                    TimeTbForEarchSub = softThird1Update(totalClass, totalAppear, listSubject[i].appear);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softThird2Update(totalClass, totalAppear, listSubject[i].appear);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    softThird3Update(totalClass, totalAppear, timeTableForEachClas, listSubject[i].Id.ToString());
                                }
                            }
                            else if (i == 3)
                            {
                                if (listSubject[i].appear == 1)
                                {
                                    TimeTbForEarchSub = softFour1Update(totalClass, totalAppear, listSubject[i].appear);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                }
                                else if (listSubject[i].appear == 2)
                                {
                                    TimeTbForEarchSub = softFour1Update(totalClass, totalAppear, listSubject[i].appear);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softFour2Update(totalClass, totalAppear, listSubject[i].appear);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                }
                                else if (listSubject[i].appear == 3)
                                {
                                    TimeTbForEarchSub = softFour1Update(totalClass, totalAppear, listSubject[i].appear);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softFour2Update(totalClass, totalAppear, listSubject[i].appear);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    softFour3Update(totalClass, totalAppear, timeTableForEachClas, listSubject[i].Id.ToString());
                                }
                            }
                            i++;
                        }
                        timeTableForTotalClas.Add(timeTableForEachClas);
                        cls++;
                    }

                    for (int ob = 0; ob < timeTableForTotalClas.Count; ob++)
                    {
                        string[,] currentTable = timeTableForTotalClas[ob];
                        for (int i = 0; i < rows; i++)
                        {
                            string cahoc = null;
                            for (int j = 0; j < cols; j++)
                            {
                                string ngayhoc = null;
                                if (currentTable[i, j] != null)
                                {
                                    switch (i)
                                    {
                                        case 0:
                                            cahoc = "Ca 1";
                                            break;
                                        case 1:
                                            cahoc = "Ca 2";
                                            break;
                                        case 2:
                                            cahoc = "Ca 3";
                                            break;
                                        case 3:
                                            cahoc = "Ca 4";
                                            break;
                                        case 4:
                                            cahoc = "Ca 5";
                                            break;
                                    }
                                    switch (j)
                                    {
                                        case 0:
                                            ngayhoc = "Thứ 2";
                                            break;
                                        case 1:
                                            ngayhoc = "Thứ 3";
                                            break;
                                        case 2:
                                            ngayhoc = "Thứ 4";
                                            break;
                                        case 3:
                                            ngayhoc = "Thứ 5";
                                            break;
                                        case 4:
                                            ngayhoc = "Thứ 6";
                                            break;
                                        case 5:
                                            ngayhoc = "Thứ 7";
                                            break;
                                    }
                                    Guid idDetail = Guid.NewGuid();
                                    Guid currenClassRoom = Guid.Empty;
                                    for (int index = 0; index < listClassRoom.Count; index++)
                                    {
                                        if (listClassRoomCheck[index][i, j] == true)
                                        {
                                            currenClassRoom = listClassRoom[index];
                                            listClassRoomCheck[index][i, j] = false;
                                            break;
                                        }
                                    }
                                    using (var connect1 = _connectToSql.CreateConnection())
                                    {
                                        SqlCommand cmd1 = new SqlCommand();
                                        cmd1.CommandType = CommandType.Text;
                                        cmd1.CommandText = "INSERT INTO Lecture_Schedule_Detail (Id,idLecture_Schedule , idSubject , dayStudy , shiftStudy, dateStart, dateEnd,idClassRoom) VALUES ( @idDetail , @idLecture , @idSubject , @dayStudy , @shiftStudy , @dateStart, @dateEnd,@idClassRoom)";
                                        cmd1.Parameters.AddWithValue("@idDetail", idDetail);
                                        cmd1.Parameters.AddWithValue("@idLecture", listIdSchedule[ob]);
                                        cmd1.Parameters.AddWithValue("@idSubject", currentTable[i, j]);
                                        cmd1.Parameters.AddWithValue("@dayStudy", ngayhoc);
                                        cmd1.Parameters.AddWithValue("@shiftStudy", cahoc);
                                        cmd1.Parameters.AddWithValue("@dateStart", startDate);
                                        cmd1.Parameters.AddWithValue("@dateEnd", endDate);
                                        cmd1.Parameters.AddWithValue("@idClassRoom", currenClassRoom);
                                        cmd1.Connection = (SqlConnection)connect1;
                                        connect1.Open();
                                        int kq1 = await cmd1.ExecuteNonQueryAsync();
                                        int test1 = kq1;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (var idclasses in schedulingInputModel.Idclasses)
                    {
                        Guid idSchedule = Guid.NewGuid();
                        listIdSchedule.Add(idSchedule);
                        using (var connect = _connectToSql.CreateConnection())
                        {
                            SqlCommand cmd = new SqlCommand();
                            cmd.CommandType = CommandType.Text;
                            cmd.CommandText = "INSERT INTO Lecture_Schedules (idLecture_Schedule,idClass, createDate) VALUES (@id,@class, @createDate)";
                            cmd.Parameters.AddWithValue("@id", idSchedule);
                            cmd.Parameters.AddWithValue("@class", idclasses);
                            cmd.Parameters.AddWithValue("@createDate", DateTime.Now);
                            cmd.Connection = (SqlConnection)connect;
                            connect.Open();
                            int kq = await cmd.ExecuteNonQueryAsync();
                            int test = kq;
                        }
                        int i = 0;
                        string[,] timeTableForEachClas = new string[rows, cols];
                        foreach (var idsubject in listSubject)
                        {
                            if (i == 0)
                            {
                                if (listSubject[i].appear == 1)
                                {
                                    TimeTbForEarchSub = softFirst1(totalClass);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                }
                                else if (listSubject[i].appear == 2)
                                {
                                    TimeTbForEarchSub = softFirst1(totalClass);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softFirst2(totalClass);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);

                                }
                                else if (listSubject[i].appear == 3)
                                {
                                    TimeTbForEarchSub = softFirst1(totalClass);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softFirst2(totalClass);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softFirst3(totalClass, flag1);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                }
                            }
                            else if (i == 1)
                            {
                                if (listSubject[i].appear == 1)
                                {
                                    TimeTbForEarchSub = softSecond1(totalClass);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                }
                                else if (listSubject[i].appear == 2)
                                {
                                    TimeTbForEarchSub = softSecond1(totalClass);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softSecond2(totalClass);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                }
                                else if (listSubject[i].appear == 3)
                                {
                                    TimeTbForEarchSub = softSecond1(totalClass);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softSecond2(totalClass);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softSecond3(totalClass, flag2);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                }
                            }
                            else if (i == 2)
                            {
                                if (listSubject[i].appear == 1)
                                {
                                    TimeTbForEarchSub = softThird1(totalClass);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                }
                                else if (listSubject[i].appear == 2)
                                {
                                    TimeTbForEarchSub = softThird1(totalClass);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softThird2(totalClass);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                }
                                else if (listSubject[i].appear == 3)
                                {
                                    TimeTbForEarchSub = softThird1(totalClass);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softThird2(totalClass);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softThird3(totalClass, flag3);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                }
                            }
                            else if (i == 3)
                            {
                                if (listSubject[i].appear == 1)
                                {
                                    TimeTbForEarchSub = softFour1(totalClass);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                }
                                else if (listSubject[i].appear == 2)
                                {
                                    TimeTbForEarchSub = softFour1(totalClass);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softFour2(totalClass);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                }
                                else if (listSubject[i].appear == 3)
                                {
                                    TimeTbForEarchSub = softFour1(totalClass);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softFour2(totalClass);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softFour3(totalClass, flag4);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                }
                            }
                            else if (i == 4)
                            {
                                if (listSubject[i].appear == 1)
                                {
                                    if (flag1 == false)
                                    {
                                        TimeTbForEarchSub = softFirst3(totalClass, flag1);
                                        fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    }
                                    else if (flag2 == false)
                                    {
                                        TimeTbForEarchSub = softSecond3(totalClass, flag1);
                                        fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    }
                                    else if (flag3 == false)
                                    {
                                        TimeTbForEarchSub = softThird3(totalClass, flag1);
                                        fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    }
                                    else if (flag4 == false)
                                    {
                                        TimeTbForEarchSub = softFour3(totalClass, flag1);
                                        fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    }
                                }

                                else if (listSubject[i].appear == 2)
                                {
                                    if (flag1 == false && flag2 == false)
                                    {
                                        TimeTbForEarchSub = softFirst3(totalClass, flag1);
                                        fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                        TimeTbForEarchSub = softSecond3(totalClass, flag1);
                                        fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    }
                                    else if (flag1 == false && flag3 == false)
                                    {
                                        TimeTbForEarchSub = softFirst3(totalClass, flag1);
                                        fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                        TimeTbForEarchSub = softSecond3(totalClass, flag1);
                                        fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    }
                                    else if (flag1 == false && flag4 == false)
                                    {
                                        TimeTbForEarchSub = softFirst3(totalClass, flag1);
                                        fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                        TimeTbForEarchSub = softFour3(totalClass, flag1);
                                        fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    }
                                    else if (flag2 == false && flag3 == false)
                                    {
                                        TimeTbForEarchSub = softSecond3(totalClass, flag1);
                                        fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                        TimeTbForEarchSub = softThird3(totalClass, flag1);
                                        fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    }
                                    else if (flag2 == false && flag4 == false)
                                    {
                                        TimeTbForEarchSub = softSecond3(totalClass, flag1);
                                        fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                        TimeTbForEarchSub = softFour3(totalClass, flag1);
                                        fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    }
                                    else if (flag3 == false && flag4 == false)
                                    {
                                        TimeTbForEarchSub = softThird3(totalClass, flag1);
                                        fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                        TimeTbForEarchSub = softFour3(totalClass, flag1);
                                        fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    }
                                }
                                else if (listSubject[i].appear == 3)
                                {
                                    if (flag1 == false && flag2 == false && flag3 == false)
                                    {
                                        TimeTbForEarchSub = softFirst3(totalClass, flag1);
                                        fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                        TimeTbForEarchSub = softSecond3(totalClass, flag1);
                                        fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                        TimeTbForEarchSub = softThird3(totalClass, flag1);
                                        fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    }
                                    else if (flag1 == false && flag3 == false && flag4 == false)
                                    {
                                        TimeTbForEarchSub = softFirst3(totalClass, flag1);
                                        fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                        TimeTbForEarchSub = softThird3(totalClass, flag1);
                                        fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                        TimeTbForEarchSub = softFour3(totalClass, flag1);
                                        fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    }
                                    else if (flag2 == false && flag3 == false && flag4 == false)
                                    {
                                        TimeTbForEarchSub = softSecond3(totalClass, flag1);
                                        fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                        TimeTbForEarchSub = softThird3(totalClass, flag1);
                                        fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                        TimeTbForEarchSub = softFour3(totalClass, flag1);
                                        fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    }
                                }
                            }
                            i++;
                        }
                        timeTableForTotalClas.Add(timeTableForEachClas);
                        cls++;
                    }

                    for (int ob = 0; ob < timeTableForTotalClas.Count; ob++)
                    {
                        string[,] currentTable = timeTableForTotalClas[ob];

                        for (int i = 0; i < rows; i++)
                        {
                            string cahoc = null;
                            for (int j = 0; j < cols; j++)
                            {
                                string ngayhoc = null;
                                if (currentTable[i, j] != null)
                                {
                                    switch (i)
                                    {
                                        case 0:
                                            cahoc = "Ca 1";
                                            break;
                                        case 1:
                                            cahoc = "Ca 2";
                                            break;
                                        case 2:
                                            cahoc = "Ca 3";
                                            break;
                                        case 3:
                                            cahoc = "Ca 4";
                                            break;
                                        case 4:
                                            cahoc = "Ca 5";
                                            break;
                                    }

                                    switch (j)
                                    {
                                        case 0:
                                            ngayhoc = "Thứ 2";
                                            break;
                                        case 1:
                                            ngayhoc = "Thứ 3";
                                            break;
                                        case 2:
                                            ngayhoc = "Thứ 4";
                                            break;
                                        case 3:
                                            ngayhoc = "Thứ 5";
                                            break;
                                        case 4:
                                            ngayhoc = "Thứ 6";
                                            break;
                                        case 5:
                                            ngayhoc = "Thứ 7";
                                            break;
                                    }

                                    Guid idDetail = Guid.NewGuid();
                                    Guid currenClassRoom = Guid.Empty;
                                    for (int index = 0; index < listClassRoom.Count; index++)
                                    {
                                        if (listClassRoomCheck[index][i, j] == true)
                                        {
                                            currenClassRoom = listClassRoom[index];
                                            listClassRoomCheck[index][i, j] = false;
                                            break;
                                        }
                                    }
                                    using (var connect1 = _connectToSql.CreateConnection())
                                    {
                                        SqlCommand cmd1 = new SqlCommand();
                                        cmd1.CommandType = CommandType.Text;
                                        cmd1.CommandText = "INSERT INTO Lecture_Schedule_Detail (Id,idLecture_Schedule , idSubject , dayStudy , shiftStudy, dateStart, dateEnd,idClassRoom) VALUES ( @idDetail , @idLecture , @idSubject , @dayStudy , @shiftStudy , @dateStart, @dateEnd,@idClassRoom)";
                                        cmd1.Parameters.AddWithValue("@idDetail", idDetail);
                                        cmd1.Parameters.AddWithValue("@idLecture", listIdSchedule[ob]);
                                        cmd1.Parameters.AddWithValue("@idSubject", currentTable[i, j]);
                                        cmd1.Parameters.AddWithValue("@dayStudy", ngayhoc);
                                        cmd1.Parameters.AddWithValue("@shiftStudy", cahoc);
                                        cmd1.Parameters.AddWithValue("@dateStart", startDate);
                                        cmd1.Parameters.AddWithValue("@dateEnd", endDate);
                                        cmd1.Parameters.AddWithValue("@idClassRoom", currenClassRoom);
                                        cmd1.Connection = (SqlConnection)connect1;
                                        connect1.Open();
                                        int kq1 = await cmd1.ExecuteNonQueryAsync();
                                        int test1 = kq1;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                // lớp học bằng phòng học
                foreach (var idclasses in schedulingInputModel.Idclasses)
                {
                    Guid idClassRooms = schedulingInputModel.IdclassRooms[cls];
                    Guid idSchedule = Guid.NewGuid();
                    listIdSchedule.Add(idSchedule);
                    using (var connect = _connectToSql.CreateConnection())
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "INSERT INTO Lecture_Schedules (idLecture_Schedule,idClass,idClassroom, createDate) VALUES (@id,@class,@classRoom, @createDate)";
                        cmd.Parameters.AddWithValue("@id", idSchedule);
                        cmd.Parameters.AddWithValue("@class", idclasses);
                        cmd.Parameters.AddWithValue("@classRoom", idClassRooms);
                        cmd.Parameters.AddWithValue("@createDate", DateTime.Now);
                        cmd.Connection = (SqlConnection)connect;
                        connect.Open();
                        int kq = await cmd.ExecuteNonQueryAsync();
                        int test = kq;
                    }
                    int i = 0;
                    string[,] timeTableForEachClas = new string[rows, cols];
                    foreach (var idsubject in listSubject)
                    {
                        if (i == 0)
                        {
                            if (listSubject[i].appear == 1)
                            {
                                TimeTbForEarchSub = softFirst1(totalClass);
                                fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                            }
                            else if (listSubject[i].appear == 2)
                            {
                                TimeTbForEarchSub = softFirst1(totalClass);
                                fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                TimeTbForEarchSub = softFirst2(totalClass);
                                fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);

                            }
                            else if (listSubject[i].appear == 3)
                            {
                                TimeTbForEarchSub = softFirst1(totalClass);
                                fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                TimeTbForEarchSub = softFirst2(totalClass);
                                fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                TimeTbForEarchSub = softFirst3(totalClass, flag1);
                                fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                            }
                        }
                        else if (i == 1)
                        {
                            if (listSubject[i].appear == 1)
                            {
                                TimeTbForEarchSub = softSecond1(totalClass);
                                fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                            }
                            else if (listSubject[i].appear == 2)
                            {
                                TimeTbForEarchSub = softSecond1(totalClass);
                                fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                TimeTbForEarchSub = softSecond2(totalClass);
                                fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                            }
                            else if (listSubject[i].appear == 3)
                            {
                                TimeTbForEarchSub = softSecond1(totalClass);
                                fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                TimeTbForEarchSub = softSecond2(totalClass);
                                fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                TimeTbForEarchSub = softSecond3(totalClass, flag2);
                                fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                            }
                        }
                        else if (i == 2)
                        {
                            if (listSubject[i].appear == 1)
                            {
                                TimeTbForEarchSub = softThird1(totalClass);
                                fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                            }
                            else if (listSubject[i].appear == 2)
                            {
                                TimeTbForEarchSub = softThird1(totalClass);
                                fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                TimeTbForEarchSub = softThird2(totalClass);
                                fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                            }
                            else if (listSubject[i].appear == 3)
                            {
                                TimeTbForEarchSub = softThird1(totalClass);
                                fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                TimeTbForEarchSub = softThird2(totalClass);
                                fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                TimeTbForEarchSub = softThird3(totalClass, flag3);
                                fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                            }
                        }
                        else if (i == 3)
                        {
                            if (listSubject[i].appear == 1)
                            {
                                TimeTbForEarchSub = softFour1(totalClass);
                                fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                            }
                            else if (listSubject[i].appear == 2)
                            {
                                TimeTbForEarchSub = softFour1(totalClass);
                                fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                TimeTbForEarchSub = softFour2(totalClass);
                                fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                            }
                            else if (listSubject[i].appear == 3)
                            {
                                TimeTbForEarchSub = softFour1(totalClass);
                                fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                TimeTbForEarchSub = softFour2(totalClass);
                                fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                TimeTbForEarchSub = softFour3(totalClass, flag4);
                                fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                            }
                        }
                        else if (i == 4)
                        {
                            if (listSubject[i].appear == 1)
                            {
                                if (flag1 == false)
                                {
                                    TimeTbForEarchSub = softFirst3(totalClass, flag1);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                }
                                else if (flag2 == false)
                                {
                                    TimeTbForEarchSub = softSecond3(totalClass, flag1);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                }
                                else if (flag3 == false)
                                {
                                    TimeTbForEarchSub = softThird3(totalClass, flag1);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                }
                                else if (flag4 == false)
                                {
                                    TimeTbForEarchSub = softFour3(totalClass, flag1);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                }
                            }
                            else if (listSubject[i].appear == 2)
                            {
                                if (flag1 == false && flag2 == false)
                                {
                                    TimeTbForEarchSub = softFirst3(totalClass, flag1);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softSecond3(totalClass, flag1);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                }
                                else if (flag1 == false && flag3 == false)
                                {
                                    TimeTbForEarchSub = softFirst3(totalClass, flag1);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softSecond3(totalClass, flag1);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                }
                                else if (flag1 == false && flag4 == false)
                                {
                                    TimeTbForEarchSub = softFirst3(totalClass, flag1);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softFour3(totalClass, flag1);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                }
                                else if (flag2 == false && flag3 == false)
                                {
                                    TimeTbForEarchSub = softSecond3(totalClass, flag1);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softThird3(totalClass, flag1);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                }
                                else if (flag2 == false && flag4 == false)
                                {
                                    TimeTbForEarchSub = softSecond3(totalClass, flag1);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softFour3(totalClass, flag1);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                }
                                else if (flag3 == false && flag4 == false)
                                {
                                    TimeTbForEarchSub = softThird3(totalClass, flag1);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softFour3(totalClass, flag1);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                }
                            }
                            else if (listSubject[i].appear == 3)
                            {
                                if (flag1 == false && flag2 == false && flag3 == false)
                                {
                                    TimeTbForEarchSub = softFirst3(totalClass, flag1);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softSecond3(totalClass, flag1);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softThird3(totalClass, flag1);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                }
                                else if (flag1 == false && flag3 == false && flag4 == false)
                                {
                                    TimeTbForEarchSub = softFirst3(totalClass, flag1);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softThird3(totalClass, flag1);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softFour3(totalClass, flag1);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                }
                                else if (flag2 == false && flag3 == false && flag4 == false)
                                {
                                    TimeTbForEarchSub = softSecond3(totalClass, flag1);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softThird3(totalClass, flag1);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softFour3(totalClass, flag1);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                }
                            }
                        }
                        i++;
                    }
                    timeTableForTotalClas.Add(timeTableForEachClas);
                    cls++;
                }

                for (int ob = 0; ob < timeTableForTotalClas.Count; ob++)
                {
                    string[,] currentTable = timeTableForTotalClas[ob];

                    for (int i = 0; i < rows; i++)
                    {
                        string cahoc = null;
                        for (int j = 0; j < cols; j++)
                        {
                            string ngayhoc = null;
                            if (currentTable[i, j] != null)
                            {
                                switch (i)
                                {
                                    case 0:
                                        cahoc = "Ca 1";
                                        break;
                                    case 1:
                                        cahoc = "Ca 2";
                                        break;
                                    case 2:
                                        cahoc = "Ca 3";
                                        break;
                                    case 3:
                                        cahoc = "Ca 4";
                                        break;
                                    case 4:
                                        cahoc = "Ca 5";
                                        break;
                                }

                                switch (j)
                                {
                                    case 0:
                                        ngayhoc = "Thứ 2";
                                        break;
                                    case 1:
                                        ngayhoc = "Thứ 3";
                                        break;
                                    case 2:
                                        ngayhoc = "Thứ 4";
                                        break;
                                    case 3:
                                        ngayhoc = "Thứ 5";
                                        break;
                                    case 4:
                                        ngayhoc = "Thứ 6";
                                        break;
                                    case 5:
                                        ngayhoc = "Thứ 7";
                                        break;
                                }

                                Guid idDetail = Guid.NewGuid();
                                using (var connect1 = _connectToSql.CreateConnection())
                                {
                                    SqlCommand cmd1 = new SqlCommand();
                                    cmd1.CommandType = CommandType.Text;
                                    cmd1.CommandText = "INSERT INTO Lecture_Schedule_Detail (Id,idLecture_Schedule , idSubject , dayStudy , shiftStudy, dateStart, dateEnd) VALUES ( @idDetail , @idLecture , @idSubject , @dayStudy , @shiftStudy , @dateStart, @dateEnd)";
                                    cmd1.Parameters.AddWithValue("@idDetail", idDetail);
                                    cmd1.Parameters.AddWithValue("@idLecture", listIdSchedule[ob]);
                                    cmd1.Parameters.AddWithValue("@idSubject", currentTable[i, j]);
                                    cmd1.Parameters.AddWithValue("@dayStudy", ngayhoc);
                                    cmd1.Parameters.AddWithValue("@shiftStudy", cahoc);
                                    cmd1.Parameters.AddWithValue("@dateStart", startDate);
                                    cmd1.Parameters.AddWithValue("@dateEnd", endDate);
                                    cmd1.Connection = (SqlConnection)connect1;
                                    connect1.Open();
                                    int kq1 = await cmd1.ExecuteNonQueryAsync();
                                    int test1 = kq1;
                                }
                            }
                        }
                    }
                }
            }

            
            
        }
        public Task<string> UpdateLecture_ScheduleManagerAsync(Guid id, Lecture_ScheduleManagerModel lecture_ScheduleManagerModel)
        {
            throw new NotImplementedException();
        }
    }
}
