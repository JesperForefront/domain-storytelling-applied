# Booking domain design

## Purpose

This proof of concept models reservations of available timeslots at premises.
It focuses on the domain rules required to accept, review, approve, reject, and
communicate a booking. It deliberately excludes user interfaces, persistence,
authentication, notifications infrastructure, and external integrations.

## Scope and approach

This repository uses specification-driven development:

1. Describe a business rule and its observable acceptance scenarios here.
2. Add a focused domain test for each scenario.
3. Implement the smallest domain behaviour that satisfies those tests.
4. Keep this document and the tests aligned whenever the domain changes.

The domain project must remain framework-independent. Persistence, transport,
and presentation concerns are outside this POC.

## Ubiquitous language

| Term | Meaning |
|---|---|
| **Booker** | The abstract base type for someone who can request a reservation. It provides first name, last name, and email address. |
| **Municipality employee (ME)** | A booker employed by the municipality. A municipality employee may be assigned an approver role. |
| **Association representative (AR)** | A booker who represents an association. |
| **Private individual (PI)** | A booker acting in a private capacity and identified by a social security number. |
| **Premises** | A bookable physical location. A premise has a name and address. |
| **Timeslot** | A specific available period at one premise that may be reserved. |
| **Reservation** | The record and lifecycle of one request by a booker to reserve one timeslot. |
| **Booking policy** | The rules that determine whether a booking may proceed automatically or requires review. |
| **Approver** | A separate approval role that represents a municipality employee authorised to approve or reject a reservation. It is not a person or a booker. |
| **Confirmation or rejection** | The outcome communicated to the booker after a reservation is decided. |

Use **premises** for the concept and `Premise` for a singular domain type if the
code uses singular class names.

## Domain model

### Booker inheritance

The people model uses class inheritance, not interfaces. The intended C#
hierarchy is:

```text
Booker (abstract)
├── MunicipalityEmployee
├── AssociationRepresentative
└── PrivateIndividual
```

`Booker` has the following required fields:

| Field | Type | Meaning |
|---|---|---|
| `fname` | `NonBlankString` | First name |
| `lname` | `NonBlankString` | Last name |
| `mail` | `EmailAddress` | Email address |

`Booker` is an abstract superclass: every municipality employee, association
representative, and private individual is a booker. This inheritance hierarchy
describes who can make reservations; it does not describe approval authority.

| Concrete subclass | Additional required field | Type | Meaning |
|---|---|---|---|
| `AssociationRepresentative` | `association` | `NonBlankString` | The association represented by the booker. |
| `PrivateIndividual` | `ssn` | `SocialSecurityNumber` | The private individual's social security number. |
| `MunicipalityEmployee` | None | N/A | A municipality-employed booker. |

### Approval role and capability

`Approver` is a composed role, not a subclass of `Booker` or
`MunicipalityEmployee`. It holds exactly one `MunicipalityEmployee`. This
makes the rule explicit: an approver must be a municipality employee, while a
municipality employee does not automatically have approval authority.

`Approver` implements a small `IReservationApprover` interface. The interface
expresses the single capability needed by a reservation: its holder may decide
a pending reservation. `Reservation.Approve` and `Reservation.Reject` accept
an `IReservationApprover`, not a `MunicipalityEmployee`.

```text
Approver ──has a──> MunicipalityEmployee
Approver ──implements──> IReservationApprover
Reservation ──accepts──> IReservationApprover
```

This is composition plus an interface: the employee remains a `Booker`, but
the `Approver` role is not one. It also permits a future, different kind of
authorised approver without changing the `Reservation` API.

### Aggregate root: Reservation

`Reservation` is the transactional consistency boundary. It represents one
booker's request for one timeslot at one premise and owns its decision state.
It has a stable identity.

**References**

- Booker identity
- Timeslot identity
- Premise identity
- The policy decision applicable when the reservation was created

**Lifecycle**

| State | Meaning |
|---|---|
| `PendingApproval` | The booking policy requires an approver's decision. |
| `Approved` | The reservation is accepted, either automatically or by an approver. |
| `Rejected` | The reservation is declined by an approver. |

**Invariants**

1. A reservation concerns exactly one booker, one timeslot, and one premise.
2. The referenced timeslot must belong to the referenced premise.
3. Only an available timeslot can be reserved.
4. A reservation requiring review starts in `PendingApproval`.
5. A reservation not requiring review is approved immediately.
6. Only a pending reservation can be approved or rejected by an approver.
7. Once approved or rejected, a reservation cannot be decided again.

### Entities

| Entity | Identity | Responsibilities |
|---|---|---|
| **Booker** | `BookerId` | Abstract entity that stores `fname`, `lname`, and `mail`. |
| **MunicipalityEmployee** | `BookerId` | Concrete booker employed by the municipality. |
| **AssociationRepresentative** | `BookerId` | Concrete booker that also stores the association represented. |
| **PrivateIndividual** | `BookerId` | Concrete booker that also stores a social security number. |
| **Premise** | `PremiseId` | Represents a bookable location and stores its name and address. |
| **Timeslot** | `TimeslotId` | Represents an available period for one premise. Stores its start time, duration, premise identity, and availability. |

`Reservation` is also an entity, but is listed separately because it is the
aggregate root.

### Roles and interfaces

| Type | Responsibility |
|---|---|
| **Approver** | Role object that wraps a `MunicipalityEmployee` and grants it approval authority. |
| **IReservationApprover** | Capability interface accepted by reservation approval and rejection operations. |

### Value objects

| Value object | Rule |
|---|---|
| **NonBlankString** | Wraps a required string and rejects `null`, an empty string, and a whitespace-only string. It is used for every required textual value. |
| **Duration** | A positive whole number of seconds. |
| **EmailAddress** | A required, non-blank email address. Its email-format validation policy is not yet specified. |
| **SocialSecurityNumber** | A required, non-blank social security number. Its format and normalisation rules are not yet specified. |
| **Address** | A required, non-blank premise address. Its structure is not yet specified. |
| **BookingPolicyDecision** | The result of applying booking policy: automatic approval or review required. |
| **ReservationOutcome** | An immutable confirmation or rejection communicated to the booker. It contains the reservation identity, booker identity, and final reservation status. |

Identifiers such as `BookerId`, `PremiseId`, `TimeslotId`, and `ReservationId`
should be strongly typed value objects when introduced. Their
underlying value is a GUID.

No domain type may expose a required value as a raw `string`. It must use a
value object that rejects `null`, empty, and whitespace-only input.

### Domain service: BookingPolicy

`BookingPolicy` evaluates a booking request using the booker, timeslot, and
premise. It returns a `BookingPolicyDecision`; it does not mutate a reservation
or send messages.

The rules that cause review are intentionally not defined yet. Until they are
specified, production behaviour must not invent a policy.

### Application responsibilities

The system/application layer coordinates the flow:

1. Receive a booker's request for an available timeslot at a premise.
2. Verify that the timeslot belongs to that premise and is available.
3. Ask `BookingPolicy` for a decision.
4. Create the `Reservation` in the appropriate state.
5. Automatically approve it or route it to an approver, as directed by policy.
6. Communicate a `ReservationOutcome` to the booker after the decision.

The system is not an entity, value object, or aggregate in this model.

`ReservationOutcome` can only be created from an approved or rejected
reservation. Pending reservations do not have an outcome and must not be
communicated as if they had been decided.

The application layer communicates an outcome through
`IReservationOutcomeSender`. This is an application boundary with one
operation that accepts a `ReservationOutcome`; delivery mechanisms such as
email or SMS are infrastructure concerns and are outside this POC. Until a
booking-policy rule is specified, application coordination accepts the
applicable `BookingPolicyDecision` rather than implementing a policy.

## Acceptance scenarios

Each numbered scenario should have a corresponding focused domain test once its
behaviour is implemented.

### Creating reservations

1. **Given** an available timeslot belonging to a premise and a valid concrete
   booker,
   **when** the policy says automatic approval, **then** creating the reservation
   results in an approved reservation.
2. **Given** an available timeslot belonging to a premise and a valid booker,
   **when** the policy says review is required, **then** creating the reservation
   results in a pending reservation.
3. **Given** a timeslot that belongs to a different premise, **when** a
   reservation is requested, **then** creation fails explicitly.
4. **Given** an unavailable timeslot, **when** a reservation is requested,
   **then** creation fails explicitly.

### Reviewing reservations

1. **Given** a pending reservation, **when** an approver approves it, **then**
   the reservation becomes approved.
2. **Given** a pending reservation, **when** an approver rejects it, **then**
   the reservation becomes rejected.
3. **Given** an approved or rejected reservation, **when** a decision is
   attempted, **then** the operation fails explicitly and the original outcome
   remains unchanged.

### Communicating reservation outcomes

1. **Given** an approved reservation, **when** an outcome is created, **then**
   it contains the reservation identity, booker identity, and approved status.
2. **Given** a rejected reservation, **when** an outcome is created, **then**
   it contains the rejected status.
3. **Given** a pending reservation, **when** an outcome is created, **then**
   creation fails explicitly.
4. **Given** an automatically approved reservation, **when** application
   coordination creates it, **then** it sends its outcome through
   `IReservationOutcomeSender`.
5. **Given** a reservation requiring review, **when** application coordination
   creates it, **then** it does not send an outcome until an approver decides it.
6. **Given** a pending reservation, **when** application coordination approves
   or rejects it, **then** it sends the corresponding outcome through
   `IReservationOutcomeSender`.

### Timeslots

1. **Given** a duration of zero or fewer seconds, **when** a timeslot or
   duration is created, **then** creation fails explicitly.
2. **Given** a valid start time, positive duration, and premise identity,
   **when** a timeslot is created, **then** it is available unless explicitly
   marked otherwise.

### People and required text

1. **Given** a `null`, empty, or whitespace-only string, **when** a
   `NonBlankString` is created, **then** creation fails explicitly.
2. **Given** non-blank first name, last name, and email address, **when** a
   concrete booker is created, **then** it exposes those values through
   `Booker`.
3. **Given** an association representative, **when** it is created, **then**
   it must have a non-blank association.
4. **Given** a private individual, **when** it is created, **then** it must
   have a non-blank social security number.
5. **Given** an approver, **when** it is created, **then** it is also a
   role for exactly one municipality employee, but is not itself a booker.
6. **Given** a municipality employee without an approver role, **when** a
   pending reservation needs a decision, **then** that employee cannot be
   supplied to the reservation as an `IReservationApprover`.

## Open decisions

The following are intentionally unresolved and must be specified before their
behaviour is implemented:

- Which booker, timeslot, and premise conditions require manual review.
- Whether an approved reservation immediately makes its timeslot unavailable,
  and how concurrent requests are resolved.
- Whether a rejected or cancelled reservation makes a timeslot available again.
- Whether reservations can be cancelled, rescheduled, or expire.
- The format and normalisation rules for email addresses, social security
  numbers, and addresses beyond the required non-blank rule.
- Who assigns or revokes the `Approver` role. Authorisation infrastructure is
  outside the domain POC.
