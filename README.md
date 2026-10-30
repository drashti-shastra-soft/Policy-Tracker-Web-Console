# Policy Tracker Web Console

Policy Tracker Web Console is a responsive web application for insurance agents to track every policy placed through the broker. Agents can record fresh and renewal policies across life, health, motor, and general insurance, match them against broker statements to catch missing entries and lost commission, and monitor upcoming renewals, premiums, and payouts in one place. The console is built with ASP.NET MVC, consumes the Policy Tracker Web API, and features role-based access and secure login.

## Features

- **Authentication** — secure sign-in with role-based access for agents and admins.
- **Dashboard** — overview of active policies, upcoming renewals, pending premiums, and commission summaries.
- **Client management** — manage client records, contact details, and policy history.
- **Policy management** — record fresh and renewal policies across Life (LIC), Health, Motor, and General insurance.
- **Broker statement matching** — upload or view broker statements and match them against recorded policies to identify missing entries.
- **Lost commission tracking** — highlight policies present in the broker statement but missing from the agent's records, indicating potential lost commission.
- **Renewal monitoring** — track upcoming renewals, due dates, and policy status.
- **Premium tracking** — monitor premium amounts, payment status, and due dates.
- **Payout tracking** — view expected payouts and commissions.
- **Reports** — agent-wise, policy-wise, and period-wise reports for renewals, premiums, and commissions.
- **Shared web UI** — Razor views, shared layout, pagination helper, and client-side validation.
- **Responsive design** — mobile-ready layout for agents using phones and tablets in the field.

> Feature descriptions are based on the controllers, domain models, services, and views present in this repository. Actual permissions and backend behavior may depend on the connected Policy Tracker Web API.

## Technology stack

- C# and .NET Framework 4.8 (or `<your target framework>`)
- ASP.NET MVC with Razor views
- Visual Studio solution (`PolicyTrackerWebConsole.sln`)
- Dependency injection configured through Unity
- HTTP client/service layer for communicating with the Policy Tracker Web API
- HTML, CSS, Bootstrap, JavaScript, and jQuery-based MVC views

## Solution structure

```text
V4/
├── PolicyTrackerWebConsole.sln
├── PolicyTrackerWebConsole/              # ASP.NET MVC web application
│   ├── Controllers/                      # MVC request handling
│   ├── Views/                            # Razor UI pages
│   ├── App_Start/                        # Routes, filters, Unity setup
│   ├── Config/                           # Environment-specific app settings
│   └── Web.config
├── PolicyTrackerWebConsole.Domain/       # Domain models and list/pagination models
├── PolicyTrackerWebConsole.Infrastructure/ # Service interfaces, helpers, constants, exceptions
└── PolicyTrackerWebConsole.Service/      # Service implementations and API communication