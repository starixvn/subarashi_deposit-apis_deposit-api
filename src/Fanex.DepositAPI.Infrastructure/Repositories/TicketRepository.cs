namespace Fanex.DepositAPI.Infrastructure.Repositories;

using System.Data;
using Application.Common.Interfaces;
using BetList.Core.Builder;
using BetList.Core.Entities;
using Constants;
using Domain.Entities;
using Domain.Errors;
using Domain.Extensions;
using Domain.Repositories;
using Domain.Results;
using Models;
using Utils;
using BetListTicket = BetList.Core.Entities.Ticket;
using Ticket = Domain.Entities.Ticket;

public class TicketRepository(
    IDbFactory dbFactory,
    IBetListExcelBuilder excelBuilder,
    IBetListHTMLBuilder htmlBuilder,
    ITicketHelper ticketHelper,
    IDateTimeProvider dateTimeProvider)
    : ITicketRepository
{
    public async Task<ServiceResult<Ticket>> GetTicketInformationAsync(long refNo, BalanceUp balanceUp)
    {
        var ticketDetailResult = await GetTicketFromDb(refNo, balanceUp);

        return ticketDetailResult.Match(
            extraInfo => ServiceResult<Ticket>.Success(BuildTicketText(ticketDetailResult.Result), extraInfo),
            ServiceResult<Ticket>.Failure);
    }

    private string[] BuildChoiceText(TicketDetail ticketDetail)
    {
        ticketHelper.HideSabaText = SabaUtils.IsHideSaba(ticketDetail.SpecialPlatform);

        var choiceBuilder = excelBuilder.CreateChoiceBuilder(ticketDetail.MainTicket.BetTypeId);
        var rtfChoiceList = choiceBuilder.RenderRTF(ticketDetail.MainTicket, ticketHelper, ticketDetail.TicketData, false, excelBuilder.RTFHelper);

        return rtfChoiceList.SelectMany(rtf => rtf.ToString()?.Split("\n")).ToArray();
    }

    private string BuildStatusText(TicketDetail ticketDetail)
    {
        var statusBuilder = excelBuilder.CreateStatusBuilder(ticketDetail.MainTicket.BetTypeId);
        var rtfStatus = statusBuilder.RenderRTF(ticketDetail.MainTicket, ticketHelper, ticketDetail.TicketData, false, excelBuilder.RTFHelper);

        var text = rtfStatus.ToString();

        return text?[..text.IndexOf('\n')];
    }

    private Ticket BuildTicketText(TicketDetail ticketDetail)
    {
        return new Ticket
        {
            SportType = ticketDetail.MainTicket.SportTypeId,
            AgentId = ticketDetail.AgentId,
            SystemId = ticketDetail.MainTicket.UserName,
            RefNo = htmlBuilder.BuildRefNo(ticketDetail.MainTicket),
            TransTime = htmlBuilder.BuildTransTime(ticketDetail.MainTicket),
            LicTxID = htmlBuilder.BuildLicenseeTxId(ticketDetail.MainTicket),
            Choice = BuildChoiceText(ticketDetail),
            Status = BuildStatusText(ticketDetail)
        };
    }

    private async Task<ServiceResult<TicketDetail>> GetTicketFromDb(long refNo, BalanceUp balanceUp)
    {
        const string SpKey = "API_BetList_RefNo";
        TicketDetail ticketDetail = null;

        using var objectDb = dbFactory.GetObjectDb(SpKey);

        objectDb.SetParameterValue(DbParam.RefNo, refNo);
        objectDb.SetParameterValue(DbParam.BalanceUpDay, balanceUp.BalanceUpDay);
        objectDb.SetParameterValue(DbParam.MoveBetsDay, balanceUp.MoveBetsDay);

        using (var reader = await objectDb.ExecuteReaderAsync())
        {
            if (reader.Read())
            {
                ticketDetail = new TicketDetail
                {
                    MainTicket = MapTicket(reader),
                    SpecialPlatform = Converter.ToInt(reader["SpecialPlatform"]),
                    AgentId = Converter.ToInt(reader[DbParam.Recommend])
                };

                if (reader.NextResult())
                {
                    ticketDetail.TicketData.AddRange(ReadReferenceTicket(reader).ToList());
                }
            }
        }

        var output = objectDb.GetOutputParams();

        var (code, message) = DbUtils.GetErrorOutputParameters(output);

        if (ticketDetail is null)
        {
            return ServiceResult<TicketDetail>.Failure(TicketError.NoInformation);
        }

        if (code == 0)
        {
            return ServiceResult<TicketDetail>.Success(ticketDetail);
        }

        var error = ServiceErrors.GetFromDbError(code, message);
        return ServiceResult<TicketDetail>.FailureWithData(error, ticketDetail);
    }

    private BetListTicket MapTicket(IDataReader reader)
    {
        var ticket = new BetListTicket
        {
            CurrencyId = Converter.ToInt(reader[DbParam.Currency]),
            CustId = Converter.ToLong(reader[DbParam.CustId])
        };

        ReadStringData(ticket, reader);
        ReadBooleanData(ticket, reader);
        ReadDateTimeData(ticket, reader);
        ReadNumericData(ticket, reader);

        return ticket;
    }

    private static IEnumerable<ITicketData> ReadReferenceTicket(IDataReader reader)
    {
        while (reader.Read())
        {
            var ticket = new TicketData
            {
                BetTeam = Converter.ToString(reader["betteam"]),
                RefNo = Converter.ToString(reader[DbParam.RefNo]),
                Status = Converter.ToString(reader["status"]),
                BetTypeId = Converter.ToInt(reader["bettype"]),
                Stake = Converter.ToDecimal(reader["stake"]),
                Odds = Converter.ToDecimal(reader["odds"])
            };

            yield return ticket;
        }
    }

    #region ReadTicketData

    private static void ReadStringData(BetListTicket ticket, IDataReader reader)
    {
        ticket.UserName = Converter.ToString(reader[DbParam.Username]);
        ticket.RefNo = Converter.ToString(reader[DbParam.RefNo]);
        ticket.MatchCode = Converter.ToString(reader["matchcode"]);
        ticket.Status = Converter.ToString(reader["status"]);
        ticket.BetTeam = Converter.ToString(reader["betteam"]);
        ticket.EventStatus = Converter.ToString(reader["betstatus"]);
        ticket.ShowTime = Converter.ToString(reader["showtime"]);
        ticket.OddsType = Converter.ToString(reader[DbParam.OddsType], "0");
        ticket.Race = Converter.ToString(reader["race"]);
        ticket.BetCheck = Converter.ToString(reader["betcheck"]);
        ticket.TransDesc = Converter.ToString(reader["transdesc"]);
        ticket.ExtraInfo = Converter.ToString(reader["extrainfo"]);
        ticket.IP = Converter.ToString(reader["ip"]);
        ticket.SiteId = Converter.ToString(reader["siteId"]);
        ticket.JackpotInfo = Converter.ToString(reader["JackpotInfo"]);
    }

    private static void ReadBooleanData(BetListTicket ticket, IDataReader reader)
    {
        ticket.IsLive = Converter.ToBoolean(reader["liveindicator"]);
        ticket.IsNeutral = Converter.ToBoolean(reader["isneutral"]);
    }

    private void ReadDateTimeData(BetListTicket ticket, IDataReader reader)
    {
        ticket.WinlostDate = Converter.ToDateTime(reader["winlostdate"], dateTimeProvider.UtcNow);
        ticket.TransDate = Converter.ToDateTime(reader["transdate"], dateTimeProvider.UtcNow);
        ticket.EventDate = Converter.ToDateTime(reader["eventdate"], dateTimeProvider.MinValue);
    }

    private static void ReadNumericData(BetListTicket ticket, IDataReader reader)
    {
        // Int
        ticket.SportTypeId = Converter.ToInt(reader["sporttype"]);
        ticket.BetTypeId = Converter.ToInt(reader["bettype"]);
        ticket.LeagueId = Converter.ToInt(reader["leagueid"]);
        ticket.HomeId = Converter.ToLong(reader["homeid"]);
        ticket.AwayId = Converter.ToLong(reader["awayid"]);
        ticket.MatchId = Converter.ToInt(reader["matchid"]);
        ticket.LiveHomeScore = Converter.ToInt(reader["livehomescore"]);
        ticket.LiveAwayScore = Converter.ToInt(reader["liveawayscore"]);
        ticket.WalletType = Converter.ToByte(reader["wallettype"]);
        ticket.OriginalBetType = Converter.ToInt(reader["originalbettype"]);
        ticket.LeagueGroupId = Converter.ToInt(reader["leagueGroupId"]);

        // Long
        ticket.TransId = Converter.ToLong(reader["transid"]);
        ticket.RefCode = Converter.ToLong(reader["refcode"]);
        ticket.BetId = Converter.ToNullableLong(reader["betid"]);
        ticket.BetdaqId = Converter.ToLong(reader["betdaqid"], -1);

        // Money
        ticket.PlayerComm = Converter.ToDecimal(reader["playercomm"]);
        ticket.Comm = Converter.ToDecimal(reader["comm"]);
        ticket.Winlost = Converter.ToDecimal(reader["winlost"]);
        ticket.Stake = Converter.ToDecimal(reader["stake"]);
        ticket.ActualStake = Converter.ToDecimal(reader["actual_stake"]);
        ticket.Odds = Converter.ToDecimal(reader["odds"]);
        ticket.MasterOdds = Converter.ToDecimal(reader["modds"]);
        ticket.Handicap1 = Converter.ToDecimal(reader["hdp1"]);
        ticket.Handicap2 = Converter.ToDecimal(reader["hdp2"]);
        ticket.OriginalStake = Converter.ToDecimal(reader["originalstake"], -1);
        ticket.OddsSpread = Converter.ToDecimal(reader["oddsspread"]);

        ticket.SettlementStatus = Converter.ToByte(reader["SettlementStatus"]);
    }

    #endregion ReadTicketData
}