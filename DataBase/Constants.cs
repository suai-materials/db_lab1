using System.Collections.Generic;

namespace DataBase;

public enum Mode
{
    Auth,
    Reg
}

public enum Role
{
    User = 1,
    Admin = 2
}

public static class Constants
{
    public static Dictionary<int, Role> RoleByInt = new() {{2, Role.Admin}, {1, Role.User}};
    public static List<string> ColumnNames = new() {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M"};
}