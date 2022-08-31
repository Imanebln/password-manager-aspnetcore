export interface RequestResetPassword{
    email: string,
    token: string,
    newPassword: string,
    confirmNewPassword: string
}