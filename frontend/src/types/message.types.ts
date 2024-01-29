export interface ISendMessageDto {
    receiverUserName: string;
    text: string;
}

export interface IMessageDto extends ISendMessageDto {
    Id: number;
    senderUsername: string;
    createdAt: string;
}