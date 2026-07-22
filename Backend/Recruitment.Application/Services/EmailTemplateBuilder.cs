using System.Globalization;
using System.Net;

namespace Recruitment.Application.Services
{
    public static class EmailTemplateBuilder
    {
        public static string Test()
            => Layout(
                "Recruitment system email test",
                """
                <p>This is a test email from TalentAI.</p>
                <p>If you received this message, SMTP email delivery is configured correctly.</p>
                """);

        public static string Welcome(string? candidateName)
            => Layout(
                "Welcome to TalentAI",
                $"""
                <p>Hello {EncodeName(candidateName)},</p>
                <p>Your TalentAI account has been created successfully.</p>
                <p>You can now complete your candidate profile, upload your CV, and apply for jobs.</p>
                """);

        public static string ApplicationSubmitted(
            string? candidateName,
            string? jobTitle,
            string? companyName,
            DateTime appliedDate)
        {
            var companyText = string.IsNullOrWhiteSpace(companyName)
                ? string.Empty
                : $" at <strong>{Encode(companyName)}</strong>";

            return Layout(
                "Application received",
                $"""
                <p>Hello {EncodeName(candidateName)},</p>
                <p>We received your application for <strong>{EncodeFallback(jobTitle, "the selected role")}</strong>{companyText}.</p>
                <p>Submitted: <strong>{FormatDate(appliedDate)}</strong></p>
                <p>We will notify you when there is an update.</p>
                """);
        }

        public static string ApplicationStatusUpdated(
            string? candidateName,
            string? jobTitle,
            string status)
            => Layout(
                "Application status updated",
                $"""
                <p>Hello {EncodeName(candidateName)},</p>
                <p>Your application for <strong>{EncodeFallback(jobTitle, "the selected role")}</strong> has been updated.</p>
                <p>New status: <strong>{Encode(status)}</strong></p>
                """);

        public static string InterviewScheduled(
            string? candidateName,
            string? jobTitle,
            DateTime start,
            DateTime end,
            string? location,
            string? calendarLink)
        {
            var locationText = string.IsNullOrWhiteSpace(location)
                ? "Online or to be confirmed"
                : Encode(location);

            var calendarButton = IsWebLink(calendarLink)
                ? $"""
                <p style="margin: 24px 0;">
                  <a href="{Encode(calendarLink)}" style="background:#2563eb;color:#ffffff;padding:12px 18px;text-decoration:none;border-radius:6px;display:inline-block;">Open calendar event</a>
                </p>
                """
                : string.Empty;

            return Layout(
                "Interview scheduled",
                $"""
                <p>Hello {EncodeName(candidateName)},</p>
                <p>Your interview for <strong>{EncodeFallback(jobTitle, "the selected role")}</strong> has been scheduled.</p>
                <p>
                  Start: <strong>{FormatDate(start)}</strong><br/>
                  End: <strong>{FormatDate(end)}</strong><br/>
                  Location: <strong>{locationText}</strong>
                </p>
                {calendarButton}
                <p>Please be ready a few minutes before the interview time.</p>
                """);
        }

        public static string OfferExtended(
            string? candidateName,
            string? jobTitle,
            string? companyName,
            string? comments)
        {
            var companyText = string.IsNullOrWhiteSpace(companyName)
                ? string.Empty
                : $" at <strong>{Encode(companyName)}</strong>";

            var commentsText = string.IsNullOrWhiteSpace(comments)
                ? string.Empty
                : $"<p>Message from the hiring team:</p><blockquote style=\"margin:16px 0;padding:12px 16px;border-left:4px solid #2563eb;background:#f8fafc;\">{Encode(comments)}</blockquote>";

            return Layout(
                "Congratulations - job offer",
                $"""
                <p>Hello {EncodeName(candidateName)},</p>
                <p>Congratulations. We are pleased to offer you the role of <strong>{EncodeFallback(jobTitle, "the selected role")}</strong>{companyText}.</p>
                {commentsText}
                <p>The recruitment team will contact you with the next steps.</p>
                """);
        }

        private static string Layout(string title, string content)
            => $"""
            <!doctype html>
            <html>
            <body style="margin:0;padding:0;background:#f3f4f6;font-family:Arial,Helvetica,sans-serif;color:#111827;">
              <table role="presentation" width="100%" cellspacing="0" cellpadding="0" style="background:#f3f4f6;padding:24px 0;">
                <tr>
                  <td align="center">
                    <table role="presentation" width="100%" cellspacing="0" cellpadding="0" style="max-width:600px;background:#ffffff;border:1px solid #e5e7eb;border-radius:8px;overflow:hidden;">
                      <tr>
                        <td style="padding:24px 28px;background:#0f172a;color:#ffffff;">
                          <div style="font-size:20px;font-weight:700;">TalentAI Recruitment</div>
                        </td>
                      </tr>
                      <tr>
                        <td style="padding:28px;">
                          <h1 style="margin:0 0 18px;font-size:24px;line-height:1.3;color:#111827;">{Encode(title)}</h1>
                          <div style="font-size:15px;line-height:1.7;color:#374151;">
                            {content}
                          </div>
                          <p style="margin-top:28px;font-size:14px;line-height:1.6;color:#6b7280;">Thank you,<br/>TalentAI Recruitment Team</p>
                        </td>
                      </tr>
                    </table>
                  </td>
                </tr>
              </table>
            </body>
            </html>
            """;

        private static string EncodeName(string? value)
            => EncodeFallback(value, "there");

        private static string EncodeFallback(string? value, string fallback)
            => Encode(string.IsNullOrWhiteSpace(value) ? fallback : value);

        private static string Encode(string? value)
            => WebUtility.HtmlEncode(value ?? string.Empty);

        private static string FormatDate(DateTime value)
            => value.ToUniversalTime().ToString(
                "dddd, dd MMM yyyy HH:mm 'UTC'",
                CultureInfo.InvariantCulture);

        private static bool IsWebLink(string? value)
            => Uri.TryCreate(value, UriKind.Absolute, out var uri)
                && (uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeHttp);
    }
}
