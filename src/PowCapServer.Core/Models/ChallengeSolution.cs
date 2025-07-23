using System.Collections.Generic;

namespace PowCapServer.Models;

public record ChallengeSolution
{
    public ChallengeSolution(string token, IList<int> solutions)
    {
        Token = token;
        Solutions = solutions;
    }

    /// <summary>
    /// Challenge token
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    /// Array of challenge solutions
    /// </summary>
    public IList<int> Solutions { get; set; }
}
