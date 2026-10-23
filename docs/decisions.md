# Badil decisions log

A decision log is one short entry for every choice you make that the design did not already make for you, or every time you change your mind. In three months you will not remember why you chose something. This file will.

## How to use it

- One entry per decision. Three to five lines. Not an essay.
- Write it when you decide, not later.
- Never delete an old entry. If you change your mind, add a new entry and mark the old one `Superseded by D-0xx`.
- Commit it with the code change it explains.

## Template

```
## D-000: Short title
- Date: YYYY-MM-DD
- Decision: what you chose
- Why: the reason, in one or two sentences
- Alternatives: what you did not choose
- Status: Accepted | Superseded by D-0xx | Open
```

---

# Decisions made while designing (before the first line of code)

## D-001: Only two roles, User and Admin
- Decision: One account type for factories. Buyer and Seller are roles inside a single deal, not account types.
- Why: A factory can both offer and demand. The first SRS wrongly split them.
- Status: Accepted

## D-002: No AI features
- Decision: Removed AI matching, AI pricing and image analysis. Matching uses keywords, filters and location.
- Why: Scope. Price is set by the seller.
- Status: Accepted

## D-003: One User account per factory
- Decision: Business data (offers, requests, deals, chat) belongs to the FactoryProfile. Account data and notifications belong to the User.
- Why: Keeps login separate from the business.
- Status: Accepted

## D-004: Chat is tied to a listing
- Decision: Every conversation is about one offer or one request. No free direct messages. A factory cannot start a chat on its own listing. One conversation per listing per initiating factory.
- Why: Keeps conversations on topic and reduces spam.
- Status: Accepted

## D-005: Prices are per unit, with several units
- Decision: Units are Kg, Ton, Liter, CubicMeter and Piece. An offer has one unit and an asking price per one unit, in EGP. A deal locks an agreed unit price. Total = quantity x agreed unit price.
- Why: Sellers sell in different units. Asking and agreed price are different things: the asking price can change later, the agreed one cannot.
- Status: Accepted

## D-006: Transport is an estimate only
- Decision: The deal shows a distance-based transport estimate. It is not part of the price or the escrow.
- Why: The platform does not handle logistics yet. It becomes an extra line on the deal later.
- Status: Accepted

## D-007: A buyer can take all or part of an offer
- Decision: An offer has a remaining quantity and an optional minimum order. Accepting a deal reserves its quantity. Cancelling returns it. A minimum order equal to the full quantity makes the offer all-or-nothing.
- Why: Buyers often need less than the whole batch.
- Status: Accepted

## D-008: Audit log from day one
- Decision: One central mechanism (a SaveChanges interceptor) writes an AuditLog row in the same transaction as every change. Reads and chat text are not logged. Passwords and tokens are never logged.
- Why: Adding it later loses history and means touching every handler.
- Status: Accepted

## D-009: Authentication
- Decision: ASP.NET Core Identity with EF Core. JWT access token of about 15 minutes. Refresh token replaced on every use, stored hashed, kept in an httpOnly cookie. Google sign-in as a second way in. Email must be confirmed before an email account can sign in. Admin accounts are seeded.
- Why: Standard, secure, and a good way to learn the hidden parts of auth.
- Status: Accepted

## D-010: Deal time windows
- Decision: Four windows (proposal, deposit, shipping, inspection). If no money is involved, the deal cancels automatically. If money is held, the deal is flagged for an admin to decide. Nothing involving money moves automatically.
- Why: Safe by default. A person decides when money is at stake.
- Status: Accepted

## D-011: Suspending a user
- Decision: Block sign-in and sessions, lock their Available offers and Open requests (lock reason OwnerSuspended), flag their active deals for admin review. Reactivating unlocks only listings locked by the suspension.
- Why: Protects the other party and keeps evidence. Lock reasons stop moderation locks from being undone by accident.
- Status: Accepted

## D-012: Environmental impact (CO2) is a future feature
- Decision: Left out of the current model. A DealCompleted event is the place it will plug in later.
- Why: Scope. Needs reliable weight conversion and trusted CO2 factors.
- Status: Accepted

## D-013: Verification is mandatory before posting
- Decision: A factory must be Approved before it can post offers or requests or start deals. The admin verification screen is part of the MVP. Until approved, a factory can browse, search and chat.
- Why: Only real factories should post. It would be pointless without an admin screen to approve them.
- Note: The SRS lists document verification as Post-MVP. The SRS needs updating.
- Status: Accepted

## D-014: Where files are stored
- Decision: Offer images go in wwwroot behind an IFileStorage interface. Verification papers go in a private folder outside wwwroot and are served only through an endpoint that checks the owner or an Admin.
- Why: Everything in wwwroot is public. Papers must not be.
- Status: Accepted

## D-015: Hazardous materials
- Decision: Out of scope. Not in the category list.
- Status: Accepted

## D-016: Public landing page and limited browsing
- Decision: Guests see a landing page and a limited view of Available offers (no price, no factory name, city only, no chat). Public endpoints are rate limited.
- Status: Accepted

## D-017: Dashboards show counts only
- Decision: Number cards from existing tables. No charts, money totals or impact figures yet.
- Status: Accepted

# Still open

## D-018: Chat transport
- Default: REST with polling first, SignalR later.
- Status: Open

## D-019: Can guests ever see prices?
- Default: hidden.
- Status: Open

---

# Your own decisions start here
