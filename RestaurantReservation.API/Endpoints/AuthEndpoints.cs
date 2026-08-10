using RestaurantReservation.API.Authentication;

namespace RestaurantReservation.API.Endpoints;

public static class AuthEndpoints
{
    public static IResult Login(JwtTokenGenerator tokenGenerator)
    {
        // Temporary test values
        var employeeId = 1;
        var role = "Manager";

        string token = tokenGenerator.GenerateToken(employeeId, role);
        return Results.Ok(new { token });
    }
}