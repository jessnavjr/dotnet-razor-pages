# One-Page Presenter Talk Track
## Legacy .NET to ASP.NET Core Razor Pages (Bank Architecture Review)

Use this as your speaker script for a 20-minute architecture board session.

## Timing Plan

- Total: 20 minutes
- Core pitch: 15 minutes
- Questions: 5 minutes

Suggested timing per slide:
1. 1.5 min
2. 1.5 min
3. 1.5 min
4. 1.5 min
5. 1.5 min
6. 1.5 min
7. 2.0 min
8. 1.5 min
9. 1.5 min
10. 1.5 min

---

## Slide 1: Decision Request and Executive Summary

Talk track:
- Today we are seeking three approvals: architecture standard, phased migration strategy, and pilot funding with KPI governance.
- This is a risk reduction and control modernization program, not only a framework migration.
- We will reduce legacy support risk while improving delivery speed and operational resilience.

Key phrase:
- The decision is whether we modernize in a controlled way now, or continue carrying growing legacy risk.

---

## Slide 2: Current-State Risk in Legacy Estate

Talk track:
- Our legacy applications show recurring patterns: inconsistent auth controls, deployment drift, weak observability, and difficult maintenance.
- In a banking context, these are not just engineering issues; they are operational and compliance risks.
- The current state creates avoidable audit friction and raises incident recovery cost.

Key phrase:
- Standing still is a risk decision with increasing cost.

---

## Slide 3: Why ASP.NET Core Razor Pages

Talk track:
- Razor Pages is the best-fit default for many secure, form-centric banking workflows.
- It gives modern security and platform support with less architecture complexity than a full SPA-first approach.
- We keep room for exceptions where high-interactivity workloads need a different frontend pattern.

Key phrase:
- Default does not mean universal; it means governed and fit-for-purpose.

---

## Slide 4: Target-State Architecture and Controls

Talk track:
- The target architecture standardizes layers: presentation, application services, data access, and integration adapters.
- We embed cross-cutting controls from day one: structured logging, centralized authorization, secure configuration, and repeatable deployment patterns.
- This reduces implementation variance and improves control consistency across applications.

Key phrase:
- Standardization is how we scale quality and control in regulated delivery.

---

## Slide 5: Security and Compliance Uplift

Talk track:
- Migration improves security posture through modern middleware, anti-forgery patterns, stronger auth policies, and better dependency lifecycle support.
- Compliance improves because we can demonstrate traceable, gated promotions and repeatable release evidence.
- This directly supports control domains like access management, change management, and monitoring.

Key phrase:
- We are upgrading both technical controls and auditability.

---

## Slide 6: Migration Strategy Recommendation

Talk track:
- We evaluated big-bang rewrite, lift-and-shift, and incremental strangler migration.
- We recommend incremental strangler because it minimizes risk, allows side-by-side validation, and enables rollback.
- It delivers value in waves instead of waiting for a full rewrite finish line.

Key phrase:
- Controlled increments are the safest path for business-critical banking systems.

---

## Slide 7: Delivery Waves and Governance Gates

Talk track:
- Wave 0 sets standards and readiness.
- Wave 1 pilot validates architecture and operations.
- Wave 2 scales by domain with reusable components.
- Wave 3 optimizes and retires legacy assets.
- Each wave has architecture, security, and operational gates before promotion.

Key phrase:
- Governance is built into each stage, not added at the end.

---

## Slide 8: Operating Model and Risk Mitigation

Talk track:
- Delivery will follow a DevSecOps model with automated quality gates and environment approvals.
- We are predefining incident playbooks and rollback procedures as part of migration readiness.
- Top risks are known and mitigated: disruption, security regression, and integration failures.

Key phrase:
- Risk is actively managed through controls, rehearsal, and rollback readiness.

---

## Slide 9: Business Value, KPIs, and Pilot Success

Talk track:
- We will measure outcomes, not assumptions: change failure rate, MTTR, deployment frequency, vulnerability SLA, and incident trend.
- Pilot success is explicit: no Sev 1 incidents in early run, faster lead time, security checks passed, rollback rehearsal proven.
- These KPIs define objective criteria for scale approval.

Key phrase:
- We scale only if evidence supports it.

---

## Slide 10: Final Ask and 90-Day Plan

Talk track:
- We request approval for the architecture standard, phased strategy, and pilot authorization.
- First 90 days are structured: readiness, pilot implementation, production cutover with hypercare.
- At day 90, we return with KPI evidence and a go/no-go recommendation for broader rollout.

Key phrase:
- Approve a controlled pilot that produces decision-grade evidence in 90 days.

---

## Q&A Preparation (5 Minutes)

Likely question: Why not keep legacy and patch?
- Patching addresses symptoms, not structural risk and control inconsistency.

Likely question: Why not full SPA everywhere?
- For many regulated workflow apps, SPA-first adds complexity without proportional control or business value.

Likely question: How do we avoid migration disruption?
- Incremental waves, side-by-side validation, release gates, and tested rollback plans.

Likely question: What if pilot fails?
- Pilot has explicit success criteria and bounded scope; failure becomes low-cost learning, not enterprise-wide risk.

Likely question: How will we prove compliance improvements?
- Through traceable pipeline evidence, standardized controls, and measurable operational KPIs.

---

## Closing Line

- We are not asking for a high-risk rewrite. We are asking for a governed modernization program that reduces enterprise risk while improving delivery outcomes.
