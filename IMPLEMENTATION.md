# Implementation order

Implement the domain from its smallest validated building blocks outward. Each
step should be completed with focused tests before beginning the next one.
Follow the specification and scenarios in [`DESIGN.md`](DESIGN.md).

## 1. Test project baseline

Confirm `dotnet test` runs successfully. Keep the existing sanity test until
the first domain test is in place, then remove it if it no longer adds value.

## 2. Required-text value object

Implement `NonBlankString` and its tests first. It must reject `null`, empty,
and whitespace-only input.

This unlocks the required textual fields used by all later types.

**Tests**

- A normal non-blank value can be created.
- `null` is rejected.
- An empty string is rejected.
- A whitespace-only string is rejected.

## 3. Identifier and specialised text value objects

Implement strongly typed GUID identifiers: `BookerId`, `PremiseId`,
`TimeslotId`, and `ReservationId`. Then add `EmailAddress`,
`SocialSecurityNumber`, and `Address`, each enforcing at least the
`NonBlankString` rule.

Do not add email or social-security-number format validation until that policy
is specified.

## 4. Booker hierarchy

Implement the abstract `Booker` base class with `fname`, `lname`, and `mail`.
Then implement the concrete subclasses:

1. `MunicipalityEmployee`
2. `AssociationRepresentative`, including its required `association`
3. `PrivateIndividual`, including its required `ssn`

**Tests**

- Each concrete subtype exposes the fields inherited from `Booker`.
- `AssociationRepresentative` rejects a blank association.
- `PrivateIndividual` rejects a blank social security number.

## 5. Premises and timeslots

Implement `Duration`, `Premise`, and `Timeslot`. A timeslot must have a start
time, a positive duration, a premise identity, and an availability state.

**Tests**

- Zero and negative durations are rejected.
- A valid timeslot is available by default.
- A timeslot retains its associated premise identity.

## 6. Approval role and capability

Implement `IReservationApprover` and `Approver`. `Approver` composes exactly
one `MunicipalityEmployee`; it is not a `Booker`. Do not use a boolean on
`MunicipalityEmployee` to represent approval authority.

**Tests**

- An approver exposes its municipality employee.
- An approver can be supplied where an `IReservationApprover` is required.
- A municipality employee on its own cannot be supplied as an
  `IReservationApprover`.

## 7. Booking-policy decision

Implement `BookingPolicyDecision` with the two outcomes:
`AutomaticApproval` and `ReviewRequired`. Define `BookingPolicy` only after a
business rule states how it selects one outcome.

Until then, tests may use an explicit `BookingPolicyDecision` rather than a
policy implementation.

## 8. Reservation aggregate

Implement `Reservation` after its dependencies exist. It owns the reservation
state and enforces these rules:

1. The timeslot belongs to the requested premise.
2. The timeslot is available.
3. Automatic approval creates an approved reservation.
4. Review required creates a pending reservation.
5. Only an `IReservationApprover` can approve or reject a pending reservation.
6. A decided reservation cannot be decided again.

Write one focused test for each rule before implementing it.

## 9. Reservation outcome and application coordination

Implement `ReservationOutcome` only when there is a caller that needs to
communicate a confirmation or rejection. Keep system coordination outside the
domain project: it loads objects, asks the policy, creates or decides the
reservation, and sends the outcome.

## Deferred work

Do not implement the following until a business rule is added to
`DESIGN.md`:

- Rules that determine when review is required.
- Approver role assignment or revocation.
- Timeslot locking, concurrency, or availability changes after approval.
- Cancellation, rescheduling, or expiry.
- Infrastructure for persistence, authentication, or sending notifications.
