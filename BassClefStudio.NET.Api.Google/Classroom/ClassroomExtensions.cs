using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.NET.Api.Google.Classroom
{
    public static class ClassroomExtensions
    {
        #region Courses

        public static async Task<IEnumerable<Course>> GetCoursesAsync(this ApiService apiService)
        {
            var result = await apiService.GetAsync("https://classroom.googleapis.com/v1/courses");
            if(result != null && result.Type == JTokenType.Object)
            {
                return (result["courses"] as JArray).Select(c => new Course((string)c["id"]));
            }
            else
            {
                throw new ApiException("Could not get result from Google API.");
            }
        }

        #endregion
        #region Posts

        public static async Task<IEnumerable<Post>> GetPostsAsync(this ApiService apiService, Course course)
        {
            var announcements = await GetAnnouncementsAsync(apiService, course);
            var assignments = await GetAssignmentsAsync(apiService, course);
            return announcements.Concat(assignments);
        }

        private static async Task<IEnumerable<Post>> GetAnnouncementsAsync(ApiService apiService, Course course)
        {
            var result = await apiService.GetAsync($"https://classroom.googleapis.com/v1/courses/{course.Id}/announcements");
            if (result != null && result.Type == JTokenType.Object)
            {
                return (result["announcements"] as JArray).Select(a => new Post((string)a["id"]));
            }
            else
            {
                throw new ApiException("Could not get result from Google API.");
            }
        }

        private static async Task<IEnumerable<Post>> GetAssignmentsAsync(ApiService apiService, Course course)
        {
            var result = await apiService.GetAsync($"https://classroom.googleapis.com/v1/courses/{course.Id}/courseWork");
            if (result != null && result.Type == JTokenType.Object)
            {
                return (result["courseWork"] as JArray).Select(a => new Post((string)a["id"]));
            }
            else
            {
                throw new ApiException("Could not get result from Google API.");
            }
        }

        #endregion
    }
}
