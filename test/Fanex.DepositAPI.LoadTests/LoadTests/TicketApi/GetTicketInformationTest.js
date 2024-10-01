import { sleep } from "k6";
import http from "k6/http";
import { Counter } from "k6/metrics";
import { Env, BaseUrlByEnv, ApiKeyByEnv } from "./Configuration.js"

export const CounterErrors = new Counter('errors');

export const options = {
    // need to config stages before starting
    //stages: [
    //    { duration: "1s", target: 2 },
    //    { duration: "8s", target: 2 },
    //    { duration: "1s", target: 0 },
    //],
    thresholds: {
        errors: ["count<1"],
    },
    noConnectionReuse: true,
};

export default function () {
    sendRequest(Env.LOCAL);
    sleep(1);
}

const DefaultDataByEnv = {};

DefaultDataByEnv[Env.LOCAL] = {
    refno: "1000000000549213779",
    lang: "en"
};

DefaultDataByEnv[Env.SIT] = {
    refno: "1000000000549213779",
    lang: "en"
};

const sendRequest = (env) => {
    const { refno, lang } = DefaultDataByEnv[env];
    const url = BaseUrlByEnv[env] + `/api/deposit/tickets/${refno}?lang=${lang}`;
    //const url = `https://localhost:7086/api/deposit/tickets/${refno}?lang=${lang}`;

    const params = {
        headers: {
            "Content-Type": "application/json",
            "ApiKey": ApiKeyByEnv[env],
        },
    };

    const res = http.get(url, params);
    const data = res.json();
    const isSuccess = res.status === 200 && data.errorCode === 0 && data.result !== null;

    CounterErrors.add(!isSuccess);
}

// macOs: K6_WEB_DASHBOARD=true K6_WEB_DASHBOARD_EXPORT="./Reports/GetTicketInformationLoadTests.html" k6 run GetTicketInformationLoadTests.js
// windows: $env:K6_WEB_DASHBOARD="true" ; $env:K6_WEB_DASHBOARD_EXPORT="./Reports/GetTicketInformationLoadTests.html" ; k6 run GetTicketInformationLoadTests.js
