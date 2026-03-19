# Common Enterprise Service Bus (ESB) Patterns

## Overview

An Enterprise Service Bus (ESB) is an integration backbone that connects applications, services, data stores, and external partners through messaging, routing, transformation, and policy enforcement.

This document highlights commonly used ESB patterns, when to apply them, trade-offs, and practical implementation guidance for enterprise systems.

---

## ESB Objectives in Enterprise Systems

Typical goals of an ESB include:

- Decouple producers from consumers
- Centralize integration concerns (routing, transformation, security, monitoring)
- Improve reliability through durable messaging and retries
- Standardize message contracts and governance
- Support both synchronous and asynchronous integration styles

---

## Pattern Categories

Common ESB patterns typically fall into these groups:

1. Routing patterns
2. Message transformation and mediation patterns
3. Reliability and delivery patterns
4. Orchestration and process patterns
5. Security and governance patterns
6. Observability and operations patterns

---

## 1. Routing Patterns

### 1.1 Content-Based Router

Routes a message to different destinations based on message content.

How it works:
- Inspects payload fields or headers
- Evaluates routing rules
- Sends message to one target endpoint

Example:
- Route payment messages by amount:
  - Amount <= 10,000 -> standard processing
  - Amount > 10,000 -> enhanced risk checks

Pros:
- Centralized routing logic
- Easy to evolve rules without changing producers

Cons:
- Router complexity can grow rapidly
- Rule changes require strong regression testing

Use when:
- Routing decisions depend on business attributes in message payloads

---

### 1.2 Message Filter

Drops or forwards messages based on criteria.

How it works:
- Applies rule predicates
- Keeps only relevant messages for downstream consumers

Example:
- Forward only AML-relevant transactions to compliance system

Pros:
- Reduces downstream load
- Improves data quality for consumers

Cons:
- Risk of accidental data loss if rules are wrong

Use when:
- Not all subscribers need every message

---

### 1.3 Recipient List

Sends one message to multiple receivers determined at runtime.

How it works:
- Router resolves dynamic recipient set
- Duplicates message to each recipient

Example:
- New customer onboarding event sent to CRM, KYC, notification, and analytics services

Pros:
- Flexible fan-out behavior
- Decouples producers from target list

Cons:
- Delivery tracking across many recipients can be complex

Use when:
- Multiple systems need the same event with runtime recipient logic

---

### 1.4 Splitter and Aggregator

Splitter breaks one message into many smaller messages; aggregator recombines related responses.

How it works:
- Splitter partitions a bulk payload
- Each part is processed independently
- Aggregator collects parts using correlation keys and timeout rules

Example:
- Bulk payment file split into transaction messages; results aggregated into settlement summary

Pros:
- Parallel processing improves throughput
- Better fault isolation per item

Cons:
- Correlation and timeout logic adds complexity

Use when:
- Processing batch or composite messages in parallel is needed

---

### 1.5 Dynamic Router

Continuously evaluates routing rules where next hop can change during processing.

How it works:
- Uses external rule store or policy engine
- Routes can change without deployment

Example:
- Route transactions by real-time regional availability or compliance policy updates

Pros:
- Highly adaptable integration flow

Cons:
- Harder to reason about execution paths

Use when:
- Routing logic must change frequently and quickly

---

## 2. Transformation and Mediation Patterns

### 2.1 Message Translator

Converts one message format into another.

How it works:
- Maps source schema to target schema
- Applies data type and field transformations

Example:
- Convert legacy fixed-width core banking message into JSON API contract

Pros:
- Isolates schema mismatch between systems

Cons:
- Mapping maintenance can become expensive

Use when:
- Integrating systems with different data contracts

---

### 2.2 Canonical Data Model

Defines a common enterprise message format used internally on the bus.

How it works:
- Each system maps local schema <-> canonical schema
- Reduces point-to-point mapping explosion

Pros:
- Standardizes enterprise messaging
- Reduces long-term integration complexity

Cons:
- Significant upfront design effort
- Governance discipline required

Use when:
- Many systems exchange similar business entities

---

### 2.3 Content Enricher

Adds missing data to a message by calling external systems or lookup stores.

How it works:
- Receives base message
- Fetches additional attributes
- Emits enriched message

Example:
- Add risk score and customer segment before fraud processing

Pros:
- Keeps producer payloads lightweight

Cons:
- Adds latency and potential dependency failures

Use when:
- Downstream consumers need context not present in original event

---

### 2.4 Normalizer

Converts multiple input formats into one normalized internal format.

How it works:
- Identifies source format
- Applies source-specific mapping
- Emits standardized format

Example:
- Normalize incoming payment messages from SWIFT, ACH, and card network feeds

Pros:
- Simplifies downstream processing

Cons:
- Central normalization layer can become a bottleneck

Use when:
- Similar business events arrive through heterogeneous sources

---

## 3. Reliability and Delivery Patterns

### 3.1 Store-and-Forward

Persists messages durably before forwarding.

How it works:
- Bus writes message to durable store
- Forwards to consumer
- Retries if destination is unavailable

Pros:
- Resilient to temporary outages
- Prevents message loss on transient failures

Cons:
- Adds I/O overhead and latency

Use when:
- Delivery reliability is more important than minimal latency

---

### 3.2 Retry with Exponential Backoff

Retries failed message delivery with increasing wait times.

How it works:
- On failure, retry after delay
- Delay grows exponentially to reduce pressure on failing dependency

Pros:
- Handles transient faults effectively

Cons:
- Poorly configured retries can amplify outages

Use when:
- External dependencies fail intermittently

---

### 3.3 Dead Letter Queue (DLQ)

Moves undeliverable messages to a quarantine queue after retry exhaustion.

How it works:
- Message fails retries/policy checks
- Sent to DLQ with error metadata
- Operators inspect and replay after remediation

Pros:
- Prevents poison messages from blocking pipeline
- Enables operational recovery workflows

Cons:
- Requires strong DLQ monitoring and replay tooling

Use when:
- You need operational control over persistent message failures

---

### 3.4 Idempotent Consumer

Ensures duplicate message processing does not create duplicate effects.

How it works:
- Consumer tracks message ID or business key
- Ignores already processed messages

Pros:
- Safe under at-least-once delivery semantics

Cons:
- Requires deduplication store and key strategy

Use when:
- Message brokers can redeliver messages

---

### 3.5 Transactional Outbox and Inbox

Outbox ensures events are published reliably with database transaction; inbox ensures deduplicated consumer processing.

How it works:
- Producer writes business state + outbox record in one DB transaction
- Publisher reads outbox and sends to bus
- Consumer stores inbox record to enforce once-per-key handling

Pros:
- Strong consistency between state changes and emitted events

Cons:
- Additional tables/processes increase complexity

Use when:
- Avoiding dual-write inconsistency is critical

---

## 4. Orchestration and Process Patterns

### 4.1 Orchestration (Central Process Manager)

A central coordinator controls multi-step business workflows.

How it works:
- Process manager invokes services in sequence/parallel
- Maintains workflow state
- Handles compensations on failures

Example:
- Loan origination workflow across credit check, underwriting, document verification, and disbursement

Pros:
- Clear end-to-end visibility and control

Cons:
- Coordinator can become tightly coupled and complex

Use when:
- Process has strict ordering and centralized business rules

---

### 4.2 Choreography (Event-Driven Collaboration)

Services react to events independently without central coordinator.

How it works:
- Service publishes domain event
- Other services subscribe and react

Pros:
- High autonomy and loose coupling

Cons:
- End-to-end flow can be difficult to trace

Use when:
- Domain boundaries are clear and teams own independent services

---

### 4.3 Saga Pattern (Long-Running Transactions)

Manages distributed transaction-like workflows using local transactions and compensating actions.

How it works:
- Step succeeds -> emit next event
- Step fails -> execute compensation for completed prior steps

Pros:
- Avoids global distributed transaction locks

Cons:
- Compensation logic is complex and domain-specific

Use when:
- Cross-service consistency is required without 2PC

---

## 5. Security and Governance Patterns

### 5.1 Policy Enforcement Point (PEP)

Central location where access and policy decisions are enforced.

How it works:
- Validates identity, scopes, and policy before forwarding
- Applies throttling, quotas, and schema validation

Pros:
- Consistent security posture across integrations

Cons:
- Misconfiguration can break multiple flows

Use when:
- Enterprise requires centralized API/message governance

---

### 5.2 Token Propagation and Claims Enrichment

Propagates security context across service hops.

How it works:
- Validates incoming token
- Passes identity claims or issues delegated token
- Adds service-level claims where needed

Pros:
- Preserves end-user context for auditing and authorization

Cons:
- Incorrect claim mapping can cause privilege issues

Use when:
- Downstream services need user context for decisions

---

### 5.3 Contract Versioning

Supports multiple message schema versions safely.

How it works:
- Versioned message contracts
- Consumers opt into compatible versions
- Deprecation policy with sunset dates

Pros:
- Enables independent release cycles

Cons:
- Version sprawl if not governed strictly

Use when:
- Multiple teams release producers and consumers independently

---

## 6. Observability and Operations Patterns

### 6.1 Correlation ID and Causation ID

Tracks message flow end-to-end across systems.

How it works:
- Every message carries correlation ID
- Child operations carry causation IDs linked to parent

Pros:
- Faster root-cause analysis and audit traceability

Cons:
- Requires strict propagation discipline

Use when:
- You need traceability across distributed flows

---

### 6.2 Distributed Tracing

Captures timing and dependency spans across services.

How it works:
- Instrument producers, ESB, and consumers
- Export spans to observability backend

Pros:
- Identifies latency bottlenecks quickly

Cons:
- Instrumentation overhead and sampling decisions required

Use when:
- Performance and reliability SLOs matter

---

### 6.3 Audit Trail and Immutable Event Logging

Maintains non-repudiable records of message handling and policy decisions.

How it works:
- Logs message metadata, decision outcomes, and timestamps
- Stores audit data in tamper-evident storage

Pros:
- Supports compliance and forensic investigations

Cons:
- Storage and retention management costs

Use when:
- Regulated domains require strict auditability

---

## Common ESB Anti-Patterns

### 1. God Bus Anti-Pattern

Issue:
- Pushing all business logic into ESB flows

Impact:
- Integration layer becomes monolithic and hard to change

Better approach:
- Keep domain logic in domain services; keep ESB focused on integration concerns

---

### 2. Shared Database as Integration Mechanism

Issue:
- Multiple systems integrate by reading/writing same tables

Impact:
- Tight coupling, change fragility, ownership confusion

Better approach:
- Integrate via APIs/events, not direct shared schema dependencies

---

### 3. No Idempotency Strategy

Issue:
- Duplicate delivery causes duplicate side effects

Impact:
- Double payments, inconsistent balances, duplicate notifications

Better approach:
- Make consumers idempotent with message IDs/business keys

---

### 4. Unbounded Retry Storms

Issue:
- Aggressive retries overwhelm degraded dependencies

Impact:
- Cascading failures across services

Better approach:
- Exponential backoff, circuit breakers, DLQ escalation

---

### 5. Missing Contract Governance

Issue:
- Producer changes payload without compatibility plan

Impact:
- Consumer breakage in production

Better approach:
- Schema registry, versioning policy, compatibility tests in CI

---

## Pattern Selection Guide

Use this quick mapping:

- Need routing by payload fields -> Content-Based Router
- Need one-to-many dynamic fan-out -> Recipient List
- Need batch fan-out/fan-in -> Splitter + Aggregator
- Need schema conversion -> Translator or Normalizer
- Need enterprise standard message shape -> Canonical Data Model
- Need stronger delivery guarantees -> Store-and-Forward + Retry + DLQ
- Need duplicate safety -> Idempotent Consumer
- Need distributed consistency -> Outbox/Inbox + Saga
- Need strict central control -> Orchestration
- Need team autonomy and loose coupling -> Choreography
- Need compliance traceability -> Correlation IDs + Immutable Audit Trail

---

## ESB Implementation Notes

Typical platform mapping:

- Azure Service Bus:
  - Queues/topics, dead-lettering, scheduled delivery
  - Strong for reliable messaging and cloud integration

- IBM MQ:
  - High reliability in mission-critical enterprise environments
  - Common in financial institutions

- MuleSoft / WSO2 / TIBCO:
  - Rich mediation, transformation, and API governance
  - Good for integration-heavy enterprises

- Apache Kafka (event backbone, not classic ESB):
  - High-throughput event streaming
  - Often used with schema registry and stream processing

- BizTalk Server (legacy/transition environments):
  - Mature integration capabilities in Microsoft-centric estates

---

## Banking-Specific ESB Pattern Applications

This section maps common banking integration scenarios to practical ESB pattern combinations.

### 1. Real-Time Payments (RTP, ACH, Wire)

Typical flow:
1. Channel submits payment request
2. ESB validates and enriches request
3. Fraud/risk services evaluate transaction
4. Payment network adapter submits transaction
5. Status events propagate to ledger, notifications, and reporting

Recommended patterns:
- Content-Based Router:
  - Route by payment rail (RTP, ACH, wire), amount, currency, corridor
- Message Translator + Normalizer:
  - Convert channel payload to canonical payment message
  - Normalize external network responses into internal status model
- Content Enricher:
  - Add risk score, sanctions flags, customer segment, cut-off window metadata
- Store-and-Forward + Retry + DLQ:
  - Preserve payment intent during transient network outages
- Idempotent Consumer:
  - Prevent duplicate debits/credits on retries
- Correlation ID + Immutable Audit Trail:
  - Ensure traceability for regulatory investigations

Key banking notes:
- Payment reference and idempotency key should be immutable and globally unique
- Settlement and posting should be event-driven to isolate network latency from customer channels

---

### 2. AML and Sanctions Screening

Typical flow:
1. Transaction event arrives on bus
2. ESB routes event to AML and sanctions engines
3. Alerts/decisions returned to case management and core banking
4. Suspicious events sent to investigation queues and reporting services

Recommended patterns:
- Recipient List:
  - Fan-out transaction events to AML, sanctions, behavioral analytics, and watchlist services
- Message Filter:
  - Forward only in-scope transactions for certain regulatory checks
- Orchestration:
  - Centralize gating decisions where hard stop/allow policies are required
- Saga:
  - Compensate or reverse transaction states if post-authorization compliance checks fail
- DLQ + Replay Runbook:
  - Prevent missed compliance checks due to processing failures

Key banking notes:
- Screening latency budgets must be explicit by transaction type
- False positive handling requires deterministic routing to case management queues

---

### 3. Customer Onboarding (KYC/CDD)

Typical flow:
1. Onboarding request enters integration layer
2. Identity verification, document verification, and risk scoring run in parallel
3. Results aggregate into a final onboarding decision
4. Customer profile, accounts, and channel entitlements are provisioned

Recommended patterns:
- Splitter + Aggregator:
  - Parallelize independent checks, aggregate decisions with timeout and quorum policy
- Recipient List:
  - Send onboarding event to CRM, core banking, notification, and analytics
- Canonical Data Model:
  - Standardize customer identity and onboarding event contracts enterprise-wide
- Outbox/Inbox:
  - Ensure profile creation and emitted events remain consistent

Key banking notes:
- Define deterministic timeout behavior (manual review vs auto-decline)
- Strong contract versioning is critical as KYC requirements evolve

---

### 4. Core Ledger Posting and Reconciliation

Typical flow:
1. Domain services emit business events (payment authorized, fee posted)
2. Ledger posting service consumes events and posts accounting entries
3. Reconciliation services compare internal/external settlement views
4. Exceptions route to operations workflows

Recommended patterns:
- Transactional Outbox:
  - Eliminate dual-write risk between service database and bus publication
- Idempotent Consumer:
  - Guarantee exactly-once effect semantics for postings
- Choreography:
  - Let independent consumers (ledger, notifications, analytics) react to financial events
- Aggregator:
  - Collect settlement confirmations and reconcile by batch or business date
- Audit Trail:
  - Maintain immutable history of posting decisions and corrections

Key banking notes:
- Posting keys and event sequence ordering must be explicit
- Reconciliation pipelines should tolerate delayed/out-of-order messages

---

### 5. Legacy Core Modernization (Strangler Pattern with ESB)

Typical flow:
1. Existing legacy endpoints remain active
2. ESB intercepts and routes selected capabilities to new services
3. Canonical events published for both legacy and new paths
4. Traffic progressively shifted until legacy capability is retired

Recommended patterns:
- Message Translator:
  - Bridge legacy fixed-format contracts with modern API/event models
- Content-Based Router:
  - Route by product, region, or customer segment during migration waves
- Contract Versioning:
  - Support coexistence of legacy and modern message formats
- Observability (correlation IDs + tracing):
  - Compare behavior across old/new paths safely

Key banking notes:
- Keep migration toggles externally configurable
- Use replayable event logs to validate parity before full cutover

---

## Banking Pattern-to-Use-Case Matrix

| Banking Use Case | Primary Patterns | Why It Fits |
|------------------|------------------|-------------|
| Instant Payments | Content-Based Router, Translator, Idempotent Consumer, DLQ | Correct rail routing, format conversion, duplicate protection, operational recovery |
| ACH Batch Processing | Splitter/Aggregator, Store-and-Forward, Retry | Parallel batch handling with durable delivery |
| Fraud and AML Screening | Recipient List, Message Filter, Orchestration, Audit Trail | Parallel screening + controlled policy decisions + compliance traceability |
| Customer Onboarding | Splitter/Aggregator, Canonical Model, Outbox/Inbox | Parallel checks with consistent downstream provisioning |
| Ledger Posting | Outbox, Idempotent Consumer, Choreography | Strong consistency and safe financial event processing |
| Legacy Modernization | Translator, Versioning, Dynamic/Content-Based Routing | Controlled migration with compatibility and reduced cutover risk |

---

## Practical Governance Checklist

For production-ready ESB operations, establish:

1. Message contract standards and versioning rules
2. Idempotency requirements per consumer
3. Retry, timeout, and DLQ policies per integration
4. Correlation ID propagation standard
5. Security baseline (authN, authZ, encryption, key rotation)
6. SLOs for latency, throughput, and failure rates
7. Runbooks for DLQ triage and replay
8. Change management with backward compatibility validation

---

## Conclusion

ESB patterns are most effective when they:

- Keep integration concerns in the integration layer
- Keep business rules in business services
- Balance reliability, performance, and operational simplicity
- Are governed with clear standards for contracts, security, and observability

A well-designed ESB using these patterns can significantly reduce coupling, improve resilience, and make enterprise integration safer to evolve over time.
