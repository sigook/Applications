import 'package:flutter/material.dart';
import 'legal_page.dart';

class PrivacyPolicyPage extends StatelessWidget {
  const PrivacyPolicyPage({super.key});

  static const _sections = [
    LegalSection(
      title: 'Introduction',
      paragraphs: [
        'SIGOOK® WORK FACTORY (dba of COVENANT GROUP INVESTORS LLC, Florida) and our affiliated COVENANT GROUP LTD., Ontario-Canada ("Company" or "We") respect your privacy and are committed to protecting it through our compliance with this policy.',
        'This policy describes how we collect, use, disclose, and protect the personal information of our customers and app users ("you") when you use SIGOOK® (collectively, "Sigook"), and our practices for collecting, using, maintaining, protecting, and disclosing that information.',
        'We will only use your personal information in accordance with this policy unless otherwise required by applicable law. We take steps to ensure that the personal information we collect about you is adequate, relevant, not excessive, and used for limited purposes.',
        'This policy applies to information we collect on Sigook, in email and other electronic messages between you and us, when you interact with our advertising on third-party services, and in person at our offices.',
        'Please read this policy carefully. If you do not agree with our policies and practices, your choice is not to use Sigook. By accessing or using Sigook, you consent to the practices described in this policy.',
        'We reserve the right to make changes to this Privacy Policy at any time. Your continued use of our Services following changes means you accept the revised Privacy Policy.',
      ],
    ),
    LegalSection(
      title: 'Information We Collect About You',
      paragraphs: [
        'We collect and use several types of information from and about you, including:',
        '• Personal information: your name, address, email, telephone number, proof of identification, work experience, police check verification, social insurance number, emergency contact, spoken languages, IP address, and any other identifier used to contact you.',
        '• Non-personal information: demographic, statistical, or aggregated data that does not directly identify you.',
        '• Technical information: login information, browser type, time zone setting, operating system, and information about your internet connection and equipment.',
        '• Non-personal details about your Sigook interactions: full URLs, clickstream data, products viewed, page response times, and interaction information.',
        '• Financial information: credit card, preauthorized debit, direct deposit or other banking information for processing payments.',
        'The information we may collect from you also includes Contact Information, Registration Information, Marketing Information, Communications, Demographic Information, Transaction Information, Financial Information, Profile Information and Uploaded Content, and other data you provide.',
      ],
    ),
    LegalSection(
      title: 'How We Collect Information About You',
      paragraphs: [
        'We use different methods to collect your information, including:',
        '• Direct interactions with you when you provide it to us, for example by filling in forms or corresponding with us.',
        '• Automated technologies or interactions as you navigate through Sigook, including cookies, web beacons, and other tracking technologies.',
        '• Third parties or publicly available sources, such as our business partners.',
        'Information collected automatically may include usage details, IP addresses, and device information. We use Cookies (browser cookies), Flash Cookies, Web Beacons, and SDKs to collect this data.',
        'We may also collect Geo-Location Information, Mobile Device Access data, Mobile Device Data, and request to send you Push Notifications. You may change our access or permissions in your device settings at any time.',
        'We honor Do Not Track (DNT) signals. If you set the Do-Not-Track signal on your browser, we will honor such signals.',
      ],
    ),
    LegalSection(
      title: 'How We Use Your Information',
      paragraphs: [
        'We use the information we collect about you to:',
        '• Present Sigook and its contents to you.',
        '• Provide you with information, products, or services you request from us.',
        '• Fulfill the purposes for which you provided the information.',
        '• Provide you with notices about your account, including expiration and renewal notices.',
        '• Carry out our obligations and enforce our rights arising from contracts with you.',
        '• Notify you about changes to Sigook or any products or services we offer.',
        '• Improve Sigook, products or services, marketing, or customer relationships.',
        '• Include you in a database of candidates for future job postings relevant to you.',
        '• Offer you open positions through our affiliated employment agencies.',
        '• Improve our Services, customize your experience, and market and advertise.',
        '• Exercise our rights, debug, and create anonymous aggregate data.',
        '• Comply with legal obligations and protect your information.',
        'We process your information based on: Performance of a contract, Legitimate commercial interest, Compliance with a legal obligation, or your Consent.',
      ],
    ),
    LegalSection(
      title: 'Disclosure of Your Information',
      paragraphs: [
        'We may disclose aggregated information about our users without restriction.',
        'We may disclose personal information we collect to:',
        '• Our subsidiaries and affiliates.',
        '• A buyer or successor in the event of a merger, divestiture, or sale of Company assets (with 30-day notice to refuse transfer).',
        '• Advertisers and advertising networks (aggregate information only; we do not disclose individually identifiable data to advertisers without your consent).',
        '• Third parties to market their products or services to you if you have not opted out.',
        '• Contractors, service providers, and other third parties we use to support our business.',
        'We may also disclose your personal information to comply with court orders, laws, or legal processes; to enforce our terms and conditions; or to protect the rights, property, or safety of the Company, our customers, or others.',
      ],
    ),
    LegalSection(
      title: 'Transferring Your Personal Information',
      paragraphs: [
        'We may transfer personal information to contractors, service providers, and other third parties we use to support our business. These parties are bound by contractual obligations to keep personal information confidential and use it only for the purposes for which we disclose it to them.',
        'We may also transfer your personal information if we are required to do so by law or legal process, to law enforcement authorities or other government officials, or when we believe disclosure is necessary or appropriate to prevent physical harm or financial loss, or in connection with an investigation of suspected or actual illegal activity.',
      ],
    ),
    LegalSection(
      title: 'Choices About How We Use and Disclose Your Information',
      paragraphs: [
        'We strive to provide you with choices regarding the personal information you provide to us. We have created mechanisms to provide you with the following control over your information:',
        '• Tracking Technologies and Advertising: You can set your browser or mobile device to refuse all or some cookies, or to alert you when cookies are being sent. Note that some parts of Sigook may not function properly if you disable cookies.',
        '• Promotional Offers from the Company: If you do not wish to receive promotional communications, you can opt out by following the unsubscribe instructions in any email we send or by updating your account settings.',
        '• Targeted Advertising: You can opt out of certain tracking by advertising networks through the Digital Advertising Alliance of Canada and USA opt-out tools.',
        'California residents may have additional rights under the California Shine the Light law and California Consumer Privacy Act (CCPA), including the right to know, right to deletion, right to opt-out, and right to non-discrimination. Contact us at appmanager@sigook.com to exercise these rights.',
        'Nevada residents have the right to opt-out of the sale of certain "covered information." We currently do not sell covered information. Contact appmanager@sigook.com if you wish to be notified of any future changes.',
      ],
    ),
    LegalSection(
      title: 'Data Security',
      paragraphs: [
        'The security of your personal information is very important to us. We use physical, electronic, and administrative measures designed to secure your personal information from accidental loss and from unauthorized access, use, alteration, and disclosure.',
        'We store all information you provide to us behind firewalls on our secure servers. Payment transactions will be encrypted using SSL technology. We use third-party digital partners (such as the App Store, Google Play Store, and Microsoft Azure) to allocate and/or host our app.',
        'The safety and security of your information also depend on you. You are responsible for keeping your password confidential. We ask you not to share your password with anyone.',
        'Unfortunately, the transmission of information via the Internet is not completely secure. Any transmission of personal information is at your own risk. We are not responsible for the circumvention of any privacy settings or security measures contained on Sigook.',
      ],
    ),
    LegalSection(
      title: 'Data Retention',
      paragraphs: [
        'Except as otherwise permitted or required by applicable law or regulation, we will only retain your personal information for as long as necessary to fulfill the purposes we collected it for, including any legal, accounting, or reporting requirements.',
        'By using the unsubscribe option in our App, you will no longer receive any advertisements, will not be entitled to receive job opportunities, nor have access to your records of employment. You may request your employment records by contacting appmanager@sigook.com.',
        'We use the following criteria to determine retention periods:',
        '• How long the data is needed to provide staffing and managed services and operate our business.',
        '• Whether there are contractual or legal obligations requiring us to retain data for a period of time.',
        '• Whether any law, statute, or regulation allows for a specific retention period.',
        '• Whether an individual has agreed to a longer retention period.',
        '• What the expectation for retention was at the time the data was provided.',
        'In all cases, we will continue to protect your personal information in accordance with the terms of this Privacy Policy.',
      ],
    ),
    LegalSection(
      title: 'Children Under the Age of 13',
      paragraphs: [
        'Sigook is not intended for children under 13 years of age. No one under age 13 may provide any information to or on Sigook.',
        'We do not knowingly collect personal information from children under 13. If you are under 13, do not use or provide any information on Sigook or register on Sigook.',
        'If we learn we have collected or received personal information from a child under 13 without verification of parental consent, we will delete that information. If you believe we might have any information from or about a child under 13, please contact us at appmanager@sigook.com.',
      ],
    ),
    LegalSection(
      title: 'Accessing and Correcting Your Personal Information',
      paragraphs: [
        'It is important that the personal information we hold about you is accurate and current. Please keep us informed if your personal information changes.',
        'By law, you have the right to request access to and to correct the personal information that we hold about you. You can review and change some of your personal information by logging into Sigook and visiting your account profile page.',
        'To review, verify, correct, or withdraw consent to the use of your personal information, send an email to appmanager@sigook.com. We cannot delete your personal information except by also deleting your user account.',
        'We may request specific information from you to help us confirm your identity and your right to access. We will inform you of reasons if we cannot provide access to your personal information, subject to any legal or regulatory restrictions.',
        'Exceptions to access include: information protected by attorney-client privilege, information that is part of a formal dispute resolution process, information about another individual that would reveal their personal information, and information that is prohibitively expensive to provide.',
      ],
    ),
    LegalSection(
      title: 'Withdrawing Your Consent',
      paragraphs: [
        'Where you have provided your consent to the collection, use, and transfer of your personal information, you may have the legal right to withdraw your consent under certain circumstances.',
        'To withdraw your consent, if applicable, contact us at appmanager@sigook.com. Please note that if you withdraw your consent, we may not be able to provide you with a particular product or service. We will explain the impact to you at the time to help you with your decision.',
      ],
    ),
    LegalSection(
      title: 'European Union General Data Protection Regulation',
      paragraphs: [
        'We exclusively market and sell our Services to United States-based organizations and individuals. Even though we do not intentionally collect information about residents of the European Union ("EU"), some EU residents\' data may be inadvertently collected through marketing channels.',
        'If you are an EU resident and would like to request that your data be securely removed from our systems, please send an email with proof of EU residency to: appmanager@sigook.com.',
        'We will remove all relevant data, so long as that removal is technically feasible, does not impact the legitimate accounting or business practices of our customers, and does not violate other regulatory or legal standards.',
      ],
    ),
    LegalSection(
      title: 'Changes to Our Privacy Policy',
      paragraphs: [
        'It is our policy to post any changes we make to our privacy policy on www.sigook.com/privacy-policy. If we make material changes to how we treat our users\' personal information, we will notify you by email to the primary email address specified in your account.',
        'We include the date the privacy policy was last revised at the top of the page. You are responsible for ensuring we have an up-to-date, active, and deliverable email address for you, and for periodically reviewing this privacy policy.',
      ],
    ),
    LegalSection(
      title: 'Contact Information',
      paragraphs: [
        'We welcome your questions, comments, and requests regarding this privacy policy and our privacy practices.',
        'Contact our Privacy Officer at:',
        'APP MANAGER\n1451 West Cypress Creek Road, Suite 300\nFort Lauderdale, FL 33309, USA\nappmanager@sigook.com',
        'We have procedures in place to receive and respond to complaints or inquiries about our handling of personal information and our compliance with this policy.',
      ],
    ),
  ];

  @override
  Widget build(BuildContext context) {
    return const LegalPage(
      title: 'Privacy Policy',
      lastUpdated: 'February 01, 2026',
      sections: _sections,
    );
  }
}
