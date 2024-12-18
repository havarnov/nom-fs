module NomFs.Mqtt.Models

type QoS =
    | AtMostOnce = 0uy
    | AtLeastOnce = 1uy
    | ExactlyOnce = 2uy
    
/// The payload of a message (Publish or Will).
///
/// As defined [3.3.2.3.2 Payload Format Indicator](https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Toc3901111) and [3.1.3.2.3 Payload Format Indicator](https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Toc3901063):
/// * 0 (0x00) Byte Indicates that the Payload is unspecified bytes, which is equivalent to not sending a Payload Format Indicator.
/// * 1 (0x01) Byte Indicates that the Payload is UTF-8 Encoded Character Data. The UTF-8 data in the Payload MUST be well-formed UTF-8 as defined by the Unicode specification.
type Payload =
    /// 0 (0x00) Byte Indicates that the Will Message is unspecified bytes, which is equivalent to not sending a Payload Format Indicator.
    | Unspecified of uint8 array

    /// 1 (0x01) Byte Indicates that the Will Message is UTF-8 Encoded Character Data. The UTF-8 data in the Payload MUST be well-formed UTF-8 as defined by the Unicode specification.
    | String of string
    
type UserProperty = {
    Key: string
    Value: string
}

/// An Application Message which is published by the Server after the Network Connection is closed in cases where the Network Connection is not closed normally.
type Will = {
    /// [3.1.2.7 Will Retain](https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Toc3901042)
    ///
    /// If retain is `false`, the Server MUST publish the Will Message as a non-retained message.
    /// If retain is `true`, the Server MUST publish the Will Message as a retained message.
    Retain: bool

    /// [3.1.2.6 Will QoS](https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Toc3901041)
    ///
    /// Specifies the QoS level to be used when publishing the Will Message.
    Qos: QoS

    /// [3.1.3.3 Will Topic](https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Toc3901069)
    ///
    /// The topic that the Will Message will be sent to.
    Topic: string

    /// [3.1.3.4 Will Payload](https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Toc3901070)
    ///
    /// The Will Payload defines the Application Message Payload that is to be published to the Will Topic.
    Payload: Payload

    /// [3.1.3.2.2 Will Delay Interval](https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Toc3901062)
    ///
    /// The Server delays publishing the Client’s Will Message until the Will Delay Interval has passed or the Session ends, whichever happens first.
    /// If a new Network Connection to this Session is made before the Will Delay Interval has passed, the Server MUST NOT send the Will Message.
    ///
    /// If the Will Delay Interval is absent, the default value is 0 and there is no delay before the Will Message is published.
    DelayInterval: uint32 option

    /// [3.1.3.2.4 Message Expiry Interval](https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Toc3901064)
    ///
    /// If present, it's the lifetime of the Will Message in seconds and is sent as the Publication Expiry Interval when the Server publishes the Will Message.
    MessageExpiryInterval: uint32 option

    /// [3.1.3.2.5 Content Type](https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Toc3901065)
    ///
    /// Describing the content of the Will Message.
    ContentType: string option

    /// [3.1.3.2.6 Response Topic](https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Toc3901066)
    ///
    /// The Topic Name for a response message (to this specific will message).
    ResponseTopic: string option

    /// [3.1.3.2.7 Correlation Data](https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Toc3901067)
    ///
    /// The Correlation Data is used by the sender of the Request Message to identify which request the Response Message is for when it is received.
    /// The value of the Correlation Data only has meaning to the sender of the Request Message and receiver of the Response Message.
    CorrelationData: uint8 array option

    /// [3.1.3.2.8 User Property](https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Toc3901068)
    UserProperties: UserProperty list option
}

type Authentication = {
    /// [3.1.2.11.9 Authentication Method](https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Toc3901055)
    ///
    /// The name of the authentication method used for extended authentication
    AuthenticationMethod: string

    /// [3.1.2.11.10 Authentication Data](https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Toc3901056)
    ///
    /// The contents of this data are defined by the authentication method.
    AuthenticationData: byte array option
}

type Connect = {
    /// [3.1.2.1 Protocol Name](https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Toc3901036)
    ///
    /// The Protocol Name is “MQTT”, capitalized as shown. The string, its offset and length will not be changed by future versions of the MQTT specification.
    ProtocolName: string

    /// [3.1.2.2 Protocol Version](https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Toc3901037)
    ///
    /// The value of the Protocol Version field for version 5.0 of the protocol is 5 (0x05).
    ProtocolVersion: uint8

    /// [3.1.3.1 Client Identifier (ClientID)](https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Toc3901059)
    ///
    /// The Client Identifier (ClientID) identifies the Client to the Server.
    /// Each Client connecting to the Server has a unique ClientID.
    /// The ClientID MUST be used by Clients and by Servers to identify state that they hold relating to this MQTT Session between the Client and the Server.
    ClientIdentifier: string

    /// [3.1.3.5 User Name](https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Toc3901071)
    ///
    /// If the username is set, it can be used by the Server for authentication and authorization.
    Username: string option

    /// [3.1.3.6 Password](https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Toc3901072)
    ///
    /// Although this field is called Password, it can be used to carry any credential information.
    Password: byte array option

    /// [3.1.2.5 Will Flag](https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Toc3901040)
    ///
    /// If the Will Flag is set to 1 this indicates that a Will Message MUST be stored on the Server and associated with the Session.
    Will: Will option

    /// [3.1.2.4 Clean Start](https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Toc3901039)
    ///
    /// If `true` the Client and Server MUST discard any existing Session and start a new Session.
    /// If `false` and there is a Session associated with the Client Identifier, the Server MUST resume communications with the Client based on state from the existing Session.
    /// If `false` and there is no Session associated with the Client Identifier, the Server MUST create a new Session.
    CleanStart: bool

    /// [3.1.2.10 Keep Alive](https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Toc3901045)
    ///
    /// It is the maximum time interval (in seconds) that is permitted to elapse between the point at which the Client finishes transmitting one MQTT Control Packet and the point it starts sending the next.
    KeepAlive: uint16

    /// [3.1.2.11.2 Session Expiry Interval](https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Toc3901048)
    ///
    /// The Client and Server MUST store the Session State after the Network Connection is closed if the Session Expiry Interval is greater than 0
    /// If the Session Expiry Interval is 0xFFFFFFFF (UINT_MAX), the Session does not expire.
    SessionExpiryInterval: uint32 option

    /// [3.1.2.11.3 Receive Maximum](https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Toc3901049)
    ///
    /// The Client uses this value to limit the number of QoS 1 and QoS 2 publications that it is willing to process concurrently.
    ReceiveMaximum: uint16 option

    /// [3.1.2.11.4 Maximum Packet Size](https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Toc3901050)
    ///
    /// If the Maximum Packet Size is not present, no limit on the packet size is imposed beyond the limitations in the protocol as a result of the remaining length encoding and the protocol header sizes.
    MaximumPacketSize: uint32 option

    /// [3.1.2.11.5 Topic Alias Maximum](https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Toc3901051)
    ///
    /// This value indicates the highest value that the Client will accept as a Topic Alias sent by the Server.
    /// The Client uses this value to limit the number of Topic Aliases that it is willing to hold on this Connection.
    TopicAliasMaximum: uint16 option

    /// [3.1.2.11.6 Request Response Information](https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Toc3901052)
    ///
    /// The Client uses this value to request the Server to return Response Information in the CONNACK.
    RequestResponseInformation: bool option

    /// [3.1.2.11.7 Request Problem Information](https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Toc3901053)
    ///
    /// The Client uses this value to indicate whether the Reason String or User Properties are sent in the case of failures.
    RequestProblemInformation: bool option

    /// [3.1.2.11.8 User Property](https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Toc3901054)
    ///
    /// User Properties on the CONNECT packet can be used to send connection related properties from the Client to the Server. The meaning of these properties is not defined by this specification.
    UserProperties: UserProperty list option

    /// [4.12 Enhanced authentication](https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Enhanced_authentication)
    ///
    /// The MQTT CONNECT packet supports basic authentication of a Network Connection using the User Name and Password fields.
    /// While these fields are named for a simple password authentication, they can be used to carry other forms of authentication such as passing a token as the Password.
    Authentication: Authentication option
}

type ConnectReason =
    /// 0 The Connection is accepted.
    | Success = 0uy
    /// 128 The Server does not wish to reveal the reason for the failure, or none of the other Reason Codes apply.
    | UnspecifiedError = 128uy
    /// 129 Data within the CONNECT packet could not be correctly parsed.
    | MalformedPacket = 129uy
    /// 130 Data in the CONNECT packet does not conform to this specification.
    | ProtocolError = 130uy
    /// 131 The CONNECT is valid but is not accepted by this Server.
    | ImplementationSpecificError = 131uy
    /// 132 The Server does not support the version of the MQTT protocol requested by the Client.
    | UnsupportedProtocolVersion = 132uy
    /// 133 The Client Identifier is a valid string but is not allowed by the Server.
    | ClientIdentifierNotValid = 133uy
    /// 134 The Server does not accept the User Name or Password specified by the Client
    | BadUserNameOrPassword = 134uy
    /// 135 The Client is not authorized to connect.
    | NotAuthorized = 135uy
    /// 136 The MQTT Server is not available.
    | ServerUnavailable = 136uy
    /// 137 The Server is busy. Try again later.
    | ServerBusy = 137uy
    /// 138 This Client has been banned by administrative action. Contact the server administrator.
    | Banned = 138uy
    /// 140 The authentication method is not supported or does not match the authentication method currently in use.
    | BadAuthenticationMethod = 140uy
    /// 144 The Will Topic Name is not malformed, but is not accepted by this Server.
    | TopicNameInvalid = 144uy
    /// 149 The CONNECT packet exceeded the maximum permissible size.
    | PacketTooLarge = 149uy
    /// 151 An implementation or administrative imposed limit has been exceeded.
    | QuotaExceeded = 151uy
    /// 153 The Will Payload does not match the specified Payload Format Indicator.
    | PayloadFormatInvalid = 153uy
    /// 154 The Server does not support retained messages, and Will Retain was set to 1.
    | RetainNotSupported = 154uy
    /// 155 The Server does not support the QoS set in Will QoS.
    | QoSNotSupported = 155uy
    /// 156 The Client should temporarily use another server.
    | UseAnotherServer = 156uy
    /// 157 The Client should permanently use another server.
    | ServerMoved = 157uy
    /// 159 The connection rate limit has been exceeded.
    | ConnectionRateExceeded = 159uy

type ConnAck = {
    /// 3.2.2.1.1 Session Present
    ///
    /// The Session Present flag informs the Client whether the Server is using Session State from a previous connection for this ClientID.
    SessionPresent: bool

    /// 3.2.2.2 Connect Reason Code
    ///
    /// If a Server sends a CONNACK packet containing a Reason code of 128 or greater it MUST then close the Network Connection.
    ConnectReason: ConnectReason

    /// 3.2.2.3.2 Session Expiry Interval
    ///
    /// Representing the Session Expiry Interval in seconds.
    /// TODO: Change to Duration?
    SessionExpiryInterval: uint32 option

    /// 3.2.2.3.3 Receive Maximum
    ///
    /// The Server uses this value to limit the number of QoS 1 and QoS 2 publications that it is willing to process concurrently for the Client.
    /// If the Receive Maximum value is absent, then its value defaults to 65,535.
    ReceiveMaximum: uint16 option

    /// 3.2.2.3.4 Maximum QoS
    ///
    /// If a Server does not support QoS 1 or QoS 2 PUBLISH packets it MUST send a Maximum QoS in the CONNACK packet specifying the highest QoS it supports.
    MaximumQos: QoS option

    /// 3.2.2.3.5 Retain Available
    ///
    /// Declares whether the Server supports retained messages.
    /// If not present, then retained messages are supported.
    RetainAvailable: bool option

    /// 3.2.2.3.6 Maximum Packet Size
    ///
    /// Representing the Maximum Packet Size the Server is willing to accept.
    /// If the Maximum Packet Size is not present, there is no limit on the packet size imposed beyond the
    /// limitations in the protocol as a result of the remaining length encoding and the protocol header sizes.
    MaximumPacketSize: uint32 option

    /// 3.2.2.3.7 Assigned Client Identifier
    ///
    /// The Client Identifier which was assigned by the Server because a zero length Client Identifier was found in the CONNECT packet.
    AssignedClientIdentifier: string option

    /// 3.2.2.3.8 Topic Alias Maximum
    ///
    /// This value indicates the highest value that the Server will accept as a Topic Alias sent by the Client.
    /// The Server uses this value to limit the number of Topic Aliases that it is willing to hold on this Connection.
    TopicAliasMaximum: uint16 option

    /// 3.2.2.3.9 Reason String
    ///
    /// The Server uses this value to give additional information to the Client.
    /// TODO: The Server MUST NOT send this property if it would increase the size of the CONNACK packet beyond the Maximum Packet Size specified by the Client
    ReasonString: string option

    /// 3.2.2.3.10 User Property
    ///
    /// The content and meaning of this property is not defined by this specification.
    /// The receiver of a CONNACK containing this property MAY ignore it.
    UserProperties: UserProperty list option

    /// 3.2.2.3.11 Wildcard Subscription Available
    ///
    /// If present, this declares whether the Server supports Wildcard Subscriptions.
    /// If not present, then Wildcard Subscriptions are supported.
    WildcardSubscriptionAvailable: bool option

    /// 3.2.2.3.12 Subscription Identifiers Available
    ///
    /// If present, this byte declares whether the Server supports Subscription Identifiers.
    /// If not present, then Subscription Identifiers are supported.
    SubscriptionIdentifiersAvailable: bool option

    /// 3.2.2.3.13 Shared Subscription Available
    ///
    /// If present, this declares whether the Server supports Shared Subscriptions.
    /// If not present, then Shared Subscriptions are supported.
    SharedSubscriptionAvailable: bool option

    /// 3.2.2.3.14 Server Keep Alive
    ///
    /// If the Server sends a Server Keep Alive on the CONNACK packet, the Client MUST use this value instead of the Keep Alive value the Client sent on CONNECT.
    /// If the Server does not send the Server Keep Alive, the Server MUST use the Keep Alive value set by the Client on CONNECT.
    ServerKeepAlive: uint16 option

    /// 3.2.2.3.15 Response Information
    ///
    /// Used as the basis for creating a Response Topic.
    ResponseInformation: string option

    /// 3.2.2.3.16 Server Reference
    ///
    /// Can be used by the Client to identify another Server to use.
    ServerReference: string option

    /// 3.2.2.3.17 Authentication Method
    ///
    /// Containing the name of the authentication method.
    AuthenticationMethod: string option

    /// 3.2.2.3.18 Authentication Data
    ///
    /// Containing the authentication data.
    /// The contents of this data are defined by the authentication method and the state of already exchanged authentication data.
    AuthenticationData: uint8 array option
}

/// [3.3 PUBLISH – Publish message](https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Toc3901100)
///
/// A PUBLISH packet is sent from a Client to a Server or from a Server to a Client to transport an Application Message.
type Publish = {
    /// [3.3.1.1 DUP](https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Toc3901102)
    ///
    /// If the DUP flag is set to 0, it indicates that this is the first occasion that the Client or
    /// Server has attempted to send this PUBLISH packet. If the DUP flag is set to 1, it indicates
    /// that this might be re-delivery of an earlier attempt to send the packet.
    Duplicate: bool

    /// [3.3.1.2 QoS](https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Toc3901103)
    ///
    /// This field indicates the level of assurance for delivery of an Application Message.
    QoS: QoS

    /// [3.3.1.3 RETAIN](https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Toc3901104)
    ///
    /// If the RETAIN flag is set to 1 in a PUBLISH packet sent by a Client to a Server, the Server
    /// MUST replace any existing retained message for this topic and store the Application Message [MQTT-3.3.1-5],
    /// so that it can be delivered to future subscribers whose subscriptions match its Topic Name.
    Retain: bool

    /// [3.3.2.1 Topic Name](https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Toc3901107)
    ///
    /// The Topic Name identifies the information channel to which Payload data is published.
    TopicName: string

    /// [3.3.2.2 Packet Identifier](https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Toc3901108)
    ///
    /// The Packet Identifier field is only present in PUBLISH packets where the QoS level is 1 or 2.
    PacketIdentifier: uint16 option

    /// [3.3.2.3.3 Message Expiry Interval](https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Toc3901112)
    ///
    /// (...) is the lifetime of the Application Message in seconds.
    MessageExpiryInterval: uint32 option

    /// [3.3.2.3.4 Topic Alias](https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Toc3901113)
    ///
    /// A Topic Alias is an integer value that is used to identify the Topic instead of using the Topic Name.
    TopicAlias:uint16 option

    /// [3.3.2.3.5 Response Topic](https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Toc3901114)
    ///
    /// (...) is used as the Topic Name for a response message.
    ResponseTopic: string option

    /// [3.3.2.3.6 Correlation Data](https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Toc3901115)
    ///
    /// The Correlation Data is used by the sender of the Request Message to identify which request
    /// the Response Message is for when it is received.
    CorrelationData: uint8 array option

    /// [3.3.2.3.7 User Property](https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Toc3901116)
    ///
    /// This property is intended to provide a means of transferring application layer name-value
    /// tags whose meaning and interpretation are known only by the application programs responsible
    /// for sending and receiving them.
    UserProperties: UserProperty list option

    /// [3.3.2.3.8 Subscription Identifier](https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Toc3901117)
    ///
    /// (...) representing the identifier of the subscription.
    /// TODO: could be multiple.
    SubscriptionIdentifier: uint32 option

    /// [3.3.2.3.9 Content Type](https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Toc3901118)
    ///
    /// (...) describing the content of the Application Message.
    ContentType: string option

    /// [3.3.3 PUBLISH Payload](https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Toc3901119)
    ///
    /// The Payload contains the Application Message that is being published.
    /// The content and format of the data is application specific.
    Payload: Payload
}

/// [3.4.2.1 PUBACK Reason Code](https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Toc3901124)
type PubAckReason =
    /// 0 - The message is accepted. Publication of the QoS 1 message proceeds.
    | Success = 0uy

    /// 16 - The message is accepted but there are no subscribers.
    /// This is sent only by the Server. If the Server knows that there are no matching subscribers,
    /// it MAY use this Reason Code instead of 0x00 (Success).
    | NoMatchingSubscribers = 16uy

    /// 128 - The receiver does not accept the publish but either does not want to reveal the reason,
    /// or it does not match one of the other values.
    | UnspecifiedError = 128uy

    /// 131 - The PUBLISH is valid but the receiver is not willing to accept it.
    | ImplementationSpecificError = 131uy

    /// 135 - The PUBLISH is not authorized.
    | NotAuthorized = 135uy

    /// 144 - The Topic Name is not malformed, but is not accepted by this Client or Server.
    | TopicNameInvalid = 144uy

    /// 145 - The Packet Identifier is already in use. This might indicate a mismatch in the
    /// Session State between the Client and Server.
    | PacketIdentifierInUse = 145uy

    /// 151 - An implementation or administrative imposed limit has been exceeded.
    | QuotaExceeded = 151uy

    /// 153 - The payload format does not match the specified Payload Format Indicator.
    | PayloadFormatInvalid = 153uy

type PubAck = {
    /// [3.4.2 PUBACK Variable Header](https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Toc3901123)
    ///
    /// (...) Packet Identifier from the PUBLISH packet that is being acknowledged, (...)
    PacketIdentifier: uint16

    /// [3.4.2.1 PUBACK Reason Code](https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Toc3901124)
    ///
    /// The Client or Server sending the PUBACK packet MUST use one of the PUBACK Reason Codes (...)
    Reason: PubAckReason

    /// [3.4.2.2.2 Reason String](https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Toc3901127)
    ///
    /// (...) This Reason String is a human readable string designed for diagnostics and is not
    /// intended to be parsed by the receiver.
    ///
    /// The sender uses this value to give additional information to the receiver. The sender MUST NOT
    /// send this property if it would increase the size of the PUBACK packet beyond the
    /// Maximum Packet Size specified by the receiver. It is a Protocol Error to include
    /// the Reason String more than once.
    ReasonString: string option

    /// [3.4.2.2.3 User Property](https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Toc3901128)
    ///
    /// (...) This property can be used to provide additional diagnostic or other information. (...)
    UserProperties: UserProperty list option
}

type RetainHandling =
    /// 0 - Send retained messages at the time of the subscribe
    | SendRetained = 0uy
    /// 1 - Send retained messages at subscribe only if the subscription does not currently exist
    | SendRetainedForNewSubscription = 1uy
    /// 2 - Do not send retained messages at the time of the subscribe
    | DoNotSendRetained = 2uy

type TopicFilter = {
    Filter: string
    MaximumQoS: QoS
    NoLocal: bool
    RetainAsPublished: bool
    RetainHandling: RetainHandling
}


type Subscribe = {
    PacketIdentifier: uint16
    
    /// The Subscription Identifier is associated with any subscription created or modified as the result of this SUBSCRIBE packet.
    /// If there is a Subscription Identifier, it is stored with the subscription.
    /// If this property is not specified, then the absence of a Subscription Identifier is stored with the subscription.
    SubscriptionIdentifier: uint32 option
    
    UserProperties: UserProperty list option
    
    TopicFilters: TopicFilter list
}

type SubscribeReason =
    /// 0
    | GrantedQoS0 = 0uy

    /// 128
    | UnspecifiedError = 128uy


type SubAck = {
    PacketIdentifier: uint16
    
    ReasonString: string option
    
    UserProperties: UserProperty list option
    
    Reasons: SubscribeReason list
}

type Unsubscribe = {
    PacketIdentifier: uint16
    
    TopicFilters: string list
    
    UserProperties: UserProperty list option
}

type UnsubscribeReason =
    | Success
    | NoSubscriptionExisted
    | UnspecifiedError
    | ImplementationSpecificError
    | NotAuthorized
    | TopicFilterInvalid
    | PacketIdentifierInUse

type UnsubAck = {
    PacketIdentifier: uint16
    ReasonString: string option
    UserProperties: UserProperty list option
    Reasons: UnsubscribeReason list
}

type DisconnectReason =
    | NormalDisconnection
    | ProtocolError
    | TopicAliasInvalid
    | SessionTakenOver

type Disconnect = {
    DisconnectReason: DisconnectReason
    SessionExpiryInterval: uint32 option
    ReasonString: string option
    UserProperties: UserProperty list option
    ServerReference: string option
}


type MqttPacket =
    | Connect of Connect
    | ConnAck of ConnAck
    | Publish of Publish
    | PubAck of PubAck
    | PubRec
    | PubRel
    | PubComp
    | Subscribe of Subscribe
    | SubAck of SubAck
    | Unsubscribe of Unsubscribe
    | UnsubAck of UnsubAck
    | PingReq
    | PingResp
    | Disconnect of Disconnect
    | Auth


type Properties = {
    /// Payload Format Indicator (0x01) - Byte
    PayloadFormatIndicator: uint8 option

    /// Message Expiry Interval (0x02) - Four Byte Integer
    UessageExpiryInterval: uint32 option

    /// Content Type (0x03) - UTF-8 Encoded String
    ContentType: string option

    /// Response Topic (0x08) - UTF-8 Encoded String
    ResponseTopic: string option

    /// Correlation Data (0x09) - Binary Data
    CorrelationData: uint8 array option

    /// Subscription Identifier (0x0B) - Variable Byte Integer
    SubscriptionIdentifier: uint32 option

    /// Session Expiry Interval (0x11) - Four Byte Integer
    SessionExpiryInterval: uint32 option

    /// Assigned Client Identifier (0x12) - UTF-8 Encoded String
    AssignedClientIdentifier: string option

    /// Server Keep Alive (0x13) - Two Byte Integer
    ServerKeepAlive: uint16 option

    /// Authentication Method (0x15) - UTF-8 Encoded String
    AuthenticationMethod: string option

    /// Authentication Data (0x16) - Binary Data
    AuthenticationData: uint8 array option

    /// Request Problem Information (0x17) - Byte
    RequestProblemInformation: bool option

    /// Will Delay Interval (0x18) - Four Byte Integer
    WillDelayInterval: uint32 option

    /// Request Response Information (0x19) - Byte
    RequestResponseInformation: bool option

    /// Response Information (0x1A) - UTF-8 Encoded String
    ResponseInformation: string option

    /// Server Reference (0x1C) - UTF-8 Encoded String
    ServerReference: string option

    /// Reason String (0x1F) - UTF-8 Encoded String
    ReasonString: string option

    /// Receive Maximum (0x21) - Two Byte Integer
    ReceiveMaximum: uint16 option

    /// Topic Alias Maximum (0x22) - Two Byte Integer
    TopicAliasMaximum: uint16 option

    /// Topic Alias (0x23) - Two Byte Integer
    TopicAlias: uint16 option

    /// Maximum QoS (0x24) - Byte
    MaximumQoS: QoS option

    /// Retain Available (0x25) - Byte
    RetainAvailable: bool option

    /// User Property (0x26) - UTF-8 String Pair
    UserProperties: UserProperty list option

    /// Maximum Packet Size (0x27) - Four Byte Integer
    MaximumPacketSize: uint32 option

    /// Wildcard Subscription Available (0x28) - Byte
    WildcardSubscriptionAvailable: bool option

    /// Subscription Identifier Available (0x29) - Byte
    SubscriptionIdentifiersAvailable: bool option

    /// Shared Subscription Available (0x2A) - Byte
    SharedSubscriptionAvailable: bool option
}
