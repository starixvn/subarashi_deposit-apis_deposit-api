using System.Data;
using BetListNPOI.SS.UserModel;
using Fanex.BetList.Core.Builder;
using Fanex.BetList.Core.Builder.ChoiceBuilder;
using Fanex.BetList.Core.Builder.StatusBuilder;
using Fanex.DepositAPI.Domain.Entities;
using Fanex.DepositAPI.Domain.Errors;
using Fanex.DepositAPI.Domain.Extensions;
using Fanex.DepositAPI.Domain.Repositories;
using Fanex.DepositAPI.Domain.Results;
using Fanex.DepositAPI.Infrastructure.Models;
using Fanex.DepositAPI.Infrastructure.Utils;

namespace Fanex.DepositAPI.Infrastructure.Repositories;

public class TicketRepository : ITicketRepository
{
    private readonly IDbFactory _dbFactory;
    private readonly IBetListExcelBuilder _excelBuilder;
    private readonly IBetListHTMLBuilder _htmlBuilder;
    private readonly BetList.Core.Entities.ITicketHelper _ticketHelper;

    public TicketRepository(
        IDbFactory dbFactory,
        IBetListExcelBuilder excelBuilder,
        IBetListHTMLBuilder htmlBuilder,
        BetList.Core.Entities.ITicketHelper ticketHelper)
    {
        _dbFactory = dbFactory;
        _excelBuilder = excelBuilder;
        _htmlBuilder = htmlBuilder;
        _ticketHelper = ticketHelper;
    }

    public async Task<ServiceResult<Ticket>> GetTicketInfomation(long refno, BalanceUp balanceUp)
    {
        var ticketDetailResult = await GetTicketFromDB(refno, balanceUp);

        return ticketDetailResult.Match(
            onSuccess: () => ServiceResult<Ticket>.Success(BuildTicketText(ticketDetailResult.Result)),
            onFailure: ServiceResult<Ticket>.Failure);
    }

    private async Task<ServiceResult<TicketDetail>> GetTicketFromDB(long refno, BalanceUp balanceUp)
    {
        const string SpKey = "API_BetList_RefNo";
        TicketDetail ticketDetail = null;

        using (var objectDb = _dbFactory.GetObjectDb(SpKey))
        {
            objectDb.SetParameterValue("Refno", refno);
            objectDb.SetParameterValue("BalanceUpDay", balanceUp.BalanceUpDay);
            objectDb.SetParameterValue("MoveBetsDay", balanceUp.MoveBetsDay);

            using (var reader = await objectDb.ExecuteReaderAsync())
            {
                if (reader.Read())
                {
                    ticketDetail = new TicketDetail
                    {
                        MainTicket = MapTicket(reader),
                        SpecialPlatform = Converter.ToInt(reader["SpecialPlatform"])
                    };

                    if (reader.NextResult())
                    {
                        ticketDetail.TicketData.AddRange(ReadReferneceTicket(reader).ToList());
                    }
                }
            }

            var output = objectDb.GetOutputParams();

            var code = Converter.ToInt(output["ErrCode"]);
            var message = Converter.ToString(output["ErrMsg"]);

            if (code != 0)
            {
                var error = ServiceErrors.GetFromDBError(code, message);
                return ServiceResult<TicketDetail>.Failure(error);
            }

            if (ticketDetail == null)
            {
                return ServiceResult<TicketDetail>.Failure(TicketError.NoInfomation);
            }

            return ServiceResult<TicketDetail>.Success(ticketDetail);
        }
    }

    private string[] BuildChoiceText(TicketDetail ticketDetail)
    {
        _ticketHelper.HideSabaText = SabaUtils.IsHideSaba(ticketDetail.SpecialPlatform);

        IChoice choiceBuilder = _excelBuilder.CreateChoiceBuilder(ticketDetail.MainTicket.BetTypeId);
        List<IRichTextString> rtfChoiceList = choiceBuilder.RenderRTF(ticketDetail.MainTicket, _ticketHelper, ticketDetail.TicketData, false, _excelBuilder.RTFHelper);

        return rtfChoiceList.SelectMany(rtf => rtf.ToString().Split("\n")).ToArray();
    }

    private string BuildStatusText(TicketDetail ticketDetail)
    {
        IStatus statusBuilder = _excelBuilder.CreateStatusBuilder(ticketDetail.MainTicket.BetTypeId);
        IRichTextString rtfStatus = statusBuilder.RenderRTF(ticketDetail.MainTicket, _ticketHelper, ticketDetail.TicketData, false, _excelBuilder.RTFHelper);

        var text = rtfStatus.ToString();

        return text[..text.IndexOf('\n')];
    }

    private Ticket BuildTicketText(TicketDetail ticketDetail)
    {
        return new Ticket
        {
            SystemId = ticketDetail.MainTicket.UserName,
            RefNo = _htmlBuilder.BuildRefNo(ticketDetail.MainTicket),
            TransTime = _htmlBuilder.BuildTransTime(ticketDetail.MainTicket),
            LicTxID = _htmlBuilder.BuildLicenseeTxId(ticketDetail.MainTicket),
            Choice = BuildChoiceText(ticketDetail),
            Status = BuildStatusText(ticketDetail),
        };
    }

    private static IEnumerable<BetList.Core.Entities.ITicketData> ReadReferneceTicket(IDataReader reader)
    {
        while (reader.Read())
        {
            var ticket = new BetList.Core.Entities.TicketData
            {
                BetTeam = Converter.ToString(reader["betteam"]),
                RefNo = Converter.ToString(reader["refno"]),
                Status = Converter.ToString(reader["status"]),
                BetTypeId = Converter.ToInt(reader["bettype"]),
                Stake = Converter.ToDecimal(reader["stake"]),
                Odds = Converter.ToDecimal(reader["odds"])
            };

            yield return ticket;
        }
    }

    private static BetList.Core.Entities.Ticket MapTicket(IDataReader reader)
    {
        var ticket = new BetList.Core.Entities.Ticket
        {
            CurrencyId = Converter.ToInt(reader["currency"]),
            CustId = Converter.ToLong(reader["custid"]),
        };

        ReadStringData(ticket, reader);
        ReadBooleanData(ticket, reader);
        ReadDateTimeData(ticket, reader);
        ReadNumbericData(ticket, reader);

        return ticket;
    }

    #region ReadTicketData

    private static void ReadStringData(BetList.Core.Entities.Ticket ticket, IDataReader reader)
    {
        ticket.UserName = Converter.ToString(reader["username"]);
        ticket.RefNo = Converter.ToString(reader["refno"]);
        ticket.MatchCode = Converter.ToString(reader["matchcode"]);
        ticket.Status = Converter.ToString(reader["status"]);
        ticket.BetTeam = Converter.ToString(reader["betteam"]);
        ticket.EventStatus = Converter.ToString(reader["betstatus"]);
        ticket.ShowTime = Converter.ToString(reader["showtime"]);
        ticket.OddsType = Converter.ToString(reader["oddstype"], "0");
        ticket.Race = Converter.ToString(reader["race"]);
        ticket.BetCheck = Converter.ToString(reader["betcheck"]);
        ticket.TransDesc = Converter.ToString(reader["transdesc"]);
        ticket.ExtraInfo = Converter.ToString(reader["extrainfo"]);
        ticket.IP = Converter.ToString(reader["ip"]);
        ticket.SiteId = Converter.ToString(reader["siteId"]);
        ticket.JackpotInfo = Converter.ToString(reader["JackpotInfo"]);
    }

    private static void ReadBooleanData(BetList.Core.Entities.Ticket ticket, IDataReader reader)
    {
        ticket.IsLive = Converter.ToBoolean(reader["liveindicator"]);
        ticket.IsNeutral = Converter.ToBoolean(reader["isneutral"]);
    }

    private static void ReadDateTimeData(BetList.Core.Entities.Ticket ticket, IDataReader reader)
    {
        ticket.WinlostDate = Converter.ToDateTime(reader["winlostdate"], DateTime.Now);
        ticket.TransDate = Converter.ToDateTime(reader["transdate"], DateTime.Now);
        ticket.EventDate = Converter.ToDateTime(reader["eventdate"], DateTime.MinValue);
    }

    private static void ReadNumbericData(BetList.Core.Entities.Ticket ticket, IDataReader reader)
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