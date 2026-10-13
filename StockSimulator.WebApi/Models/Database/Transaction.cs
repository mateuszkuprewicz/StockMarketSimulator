using System.ComponentModel.DataAnnotations.Schema;
using StockSimulator.Models.Database;

namespace StockSimulator.Models.Database;
using System;


public class Transaction
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public Company? Company { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public DateTime Date { get; set; }
    
    public string? UserSellerId { get; set; }
    public User? UserSeller { get; set; }
    
    public string? UserBuyerId { get; set; }
    public User? UserBuyer { get; set; }
    
    // public TransactorView GetSeller()
    // {
    //     return UserSellerId != null ? new UserTransactorView(UserSellerId, UserSeller) : new CompanyTransactorView(CompanyId, Company);
    // }

    // public TransactorView GetBuyer()
    // {
    //     return UserBuyerId != null ? new UserTransactorView(UserBuyerId, UserBuyer) : new CompanyTransactorView(CompanyId, Company);
    // }
}
//
// public abstract class TransactorView
// {
//     public abstract string Type { get; }
//     public abstract string Id { get; }
// }
//
// public class CompanyTransactorView(int companyId, Company? company) : TransactorView
// {
//     public override string Type => "Company";
//     public override string Id { get; } = companyId.ToString();
//     public Company? Company { get; set; } = company;
// }
//
// public class UserTransactorView(string userId, User? user) : TransactorView
// {
//     public override string Type => "User";
//     public override string Id { get; } = userId;
//     public User? User { get; set; } = user;
// }