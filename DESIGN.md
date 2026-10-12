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
| **Booker** | A municipality employee, association representative, or private individual who requests a reservation. |
| **Premises** | A bookable physical location. A premise has a name and address. |
| **Timeslot** | A specific available period at one premise that may be reserved. |
| **Reservation** | The record and lifecycle of one request by a booker to reserve one timeslot. |
| **Booking policy** | The rules that determine whether a booking may proceed automatically or requires review. |
| **Approver** | A person authorised to approve or reject a reservation that requires review. |
| **Confirmation or rejection** | The outcome communicated to the booker after a reservation is decided. |

Use **premises** for the concept and `Premise` for a singular domain type if the
code uses singular class names.

## Domain model

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
| **Booker** | `BookerId` | Represents the person making a booking request. Stores name, email address, and phone number. |
| **Premise** | `PremiseId` | Represents a bookable location and stores its name and address. |
| **Timeslot** | `TimeslotId` | Represents an available period for one premise. Stores its start time, duration, premise identity, and availability. |
| **Approver** | `ApproverId` | Represents a person authorised to decide reservations that require review. |

`Reservation` is also an entity, but is listed separately because it is the
aggregate root.

### Value objects

| Value object | Rule |
|---|---|
| **Duration** | A positive whole number of seconds. |
| **EmailAddress** | A booker's email address; its exact validation policy is not yet specified. |
| **PhoneNumber** | A booker's telephone number; its exact validation policy is not yet specified. |
| **Address** | A premise address. Its structure is not yet specified. |
| **BookingPolicyDecision** | The result of applying booking policy: automatic approval or review required. |
| **ReservationOutcome** | An immutable confirmation or rejection communicated to the booker. |

Identifiers such as `BookerId`, `PremiseId`, `TimeslotId`, `ReservationId`, and
`ApproverId` should be strongly typed value objects when introduced. Their
underlying value is a GUID.

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

## Acceptance scenarios

Each numbered scenario should have a corresponding focused domain test once its
behaviour is implemented.

### Creating reservations

1. **Given** an available timeslot belonging to a premise and a valid booker,
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

### Timeslots

1. **Given** a duration of zero or fewer seconds, **when** a timeslot or
   duration is created, **then** creation fails explicitly.
2. **Given** a valid start time, positive duration, and premise identity,
   **when** a timeslot is created, **then** it is available unless explicitly
   marked otherwise.

## Open decisions

The following are intentionally unresolved and must be specified before their
behaviour is implemented:

- Which booker, timeslot, and premise conditions require manual review.
- Whether an approved reservation immediately makes its timeslot unavailable,
  and how concurrent requests are resolved.
- Whether a rejected or cancelled reservation makes a timeslot available again.
- Whether reservations can be cancelled, rescheduled, or expire.
- The validation and normalisation rules for names, email addresses, phone
  numbers, and addresses.
- How approver authorisation is established; authorisation infrastructure is
  outside the domain POC.
