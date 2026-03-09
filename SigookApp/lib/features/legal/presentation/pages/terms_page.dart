import 'package:flutter/material.dart';
import 'legal_page.dart';

class TermsPage extends StatelessWidget {
  const TermsPage({super.key});

  static const _sections = [
    LegalSection(
      title: 'Agreement',
      paragraphs: [
        'Welcome to SIGOOK® Work Factory ("SIGOOK") website, apps, and online services offerings. This page states the Terms of Services and Conditions ("Terms") applicable when using any of the services offered by SIGOOK.',
        'Please review the Terms below carefully as they contain important information about your legal rights, remedies and obligations, which you agree to comply with and be bound by when accessing or using this website and/or any of the services offered by SIGOOK.',
        'These Terms constitute a legally binding agreement between you and SIGOOK governing access to and use of the SIGOOK Platform (the Site, Application, and Services collectively).',
        'This Agreement is governed by the laws of the State of Florida. The sole and exclusive venue for any and all issues, claims, or causes of action arising from or related to this Agreement shall be Miami Dade County, Florida.',
        'If any provision of the Terms is held to be invalid or unenforceable, the remaining provisions will continue in full force and effect.',
      ],
    ),
    LegalSection(
      title: 'Trademark',
      paragraphs: [
        'SIGOOK® Work Factory ("SIGOOK") name and logo and the names and logos of SIGOOK.com are the property of COVENANT GROUP INVESTORS LLC., Florida, United States.',
        'All other trademarks, logos and service marks appearing on the Site are trademarks of their respective owners. Nothing contained on the Site should be construed as granting any license or right to use any trademark displayed on the Site without the written permission of its respective owner.',
        'SIGOOK neither warrants nor represents that your use of materials displayed on the Site will not infringe the rights of third parties not owned by or affiliated with SIGOOK.',
        'SIGOOK respects the intellectual property of others. If you believe that your work has been copied in a way that constitutes copyright infringement, please provide SIGOOK\'s Copyright Agent the relevant information at: appmanager@sigook.com, 1451 West Cypress Creek Road, Suite 300, Fort Lauderdale, FL 33309, USA.',
      ],
    ),
    LegalSection(
      title: 'Scope of Services',
      paragraphs: [
        'SIGOOK Platform enables registered users (Members seeking employment) and affiliated agencies (COVENANT GROUP INVESTORS LLC. Florida and COVENANT GROUP LTD., Ontario) to publish Employment Opportunities and communicate and contract directly with Members.',
        'In the United States, SIGOOK acts as an agent for Job Seekers and COVENANT GROUP INVESTORS LLC., Florida. In Canada, only COVENANT GROUP LTD., Ontario acts as agent.',
        'No Guarantee: SIGOOK (a) does not guarantee you will receive any employment or job offers; (b) will not be responsible for any job offers or listings, initial screenings, or hiring decisions; and (c) is neither your employer nor your agent based solely on your usage of the Sites.',
        'Authorization: You authorize SIGOOK, Customers, and their respective agents to make investigations into your work and educational history as necessary in arriving at a decision to place you on a job.',
        'You certify the accuracy of the Candidate\'s Information in any resume or other work history information. Any misstatement of fact may cause you to be refused a job or lose your job once placed.',
      ],
    ),
    LegalSection(
      title: 'User Submission',
      paragraphs: [
        'By submitting information ("Material Information") to the SIGOOK Platform, Job Seekers and Employers grant SIGOOK a perpetual, non-exclusive, and irrevocable worldwide licence to use, copy, modify, display, distribute, and transmit such information.',
        'SIGOOK does not screen or monitor Material Information submitted by its users and makes no representation regarding its reliability, accuracy, or truthfulness. It will be validated by the staff team of the employers\' members of the Platform.',
        'SIGOOK reserves the right to delete, remove, and refuse to display or block any Submitted Material Information it considers unacceptable.',
        'User Contributions must comply with all applicable laws and must not contain defamatory, obscene, abusive, offensive, or harassing content; promote illegal activity; infringe intellectual property rights; involve impersonation; or give a false impression of endorsement.',
      ],
    ),
    LegalSection(
      title: 'Account Registration and Password',
      paragraphs: [
        'You must register an account to access and use the SIGOOK Platform. It is your sole responsibility to: (a) maintain the confidentiality of your account credentials; (b) frequently update and revise your password; and (c) promptly notify SIGOOK of any unauthorized use of your account.',
        'You must provide accurate, current, and complete information during registration and keep your SIGOOK Account information up-to-date at all times.',
        'You may not register more than one (1) SIGOOK Account unless SIGOOK authorizes you to do so.',
        'Prohibitions: You must not misuse SIGOOK\'s services. You will not build a competitive service, reverse engineer the Services, set up accounts impersonating others, commit or encourage criminal offenses, transmit viruses or malicious code, hack into any aspect of the Service, or send unsolicited advertising.',
        'Additional prohibited activities include: impersonating any person or entity; stalking or threatening; posting fraudulent or illegal information; using robots, spiders, or scrapers; harvesting user information; and causing any third party to engage in the restricted activities above.',
        'Suspension and Termination: We may suspend your right to access or use any portion or all the Services immediately upon notice if your use may subject us to liability or if you are in breach of this Agreement. We may terminate this Agreement at any time and for any reason, immediately, without notice or liability.',
      ],
    ),
    LegalSection(
      title: 'Employment',
      paragraphs: [
        'In Canada: SIGOOK does not warrant that you will receive any employment or job offers through the Site; shall not be responsible for any employment offers, screenings, or decisions; and is neither your employer nor your agent unless you enter into an express written employment agreement with COVENANT GROUP LTD., Ontario.',
        'In the United States: SIGOOK platform is the online tool used by COVENANT GROUP INVESTORS LLC., Florida (dba SIGOOK Work Factory) to offer employment. SIGOOK does not warrant that you will receive any employment or job offers; shall not be responsible for employment offers or decisions; and is neither your employer nor your agent unless you enter into an express written employment agreement with COVENANT GROUP INVESTORS LLC., Florida.',
        'In either territory, Job Seekers must get contact with the pool of recruiters to start the process of screening, recruiting, and hiring.',
        'Payments: If you wish to pay an outstanding invoice through the Sites, you may be asked to supply bank or credit card information. YOU REPRESENT AND WARRANT THAT YOU HAVE THE LEGAL RIGHT TO USE ANY PAYMENT METHOD utilized in connection with any Transaction.',
        'Direct Deposit: Employees or Independent Contractors may receive payment of wages using direct deposit. You are responsible for maintaining the accuracy of your payment information.',
      ],
    ),
    LegalSection(
      title: 'Links to Third Party',
      paragraphs: [
        'The Site may have links directing access to third party\'s websites ("Linked Sites"). SIGOOK shall not be responsible for any materials, information, or content posted on the Linked Sites. You are solely responsible for your access to the Linked Sites and should use your own judgment when using said Linked Sites.',
        'SIGOOK\'s website and apps may provide social media features that enable you to link to certain content or send communications with links to Sigook content. You may use these features solely as they are provided by us.',
        'You must not: Establish a link from any website that is not owned by you; cause the Website to appear on any other site through framing or deep linking; link to any part of the Website other than the homepage; or take any action inconsistent with these Terms of Use.',
        'We may use third-party software to serve ads, implement email marketing campaigns, and manage other interactive marketing. We may also partner with third-party vendors such as Google Analytics to analyze and track users\' use of our platforms.',
      ],
    ),
    LegalSection(
      title: 'Service Fees',
      paragraphs: [
        'SIGOOK doesn\'t charge fees for the use of the platform itself. However, this may change depending on the competitive environment.',
        'Any applicable Service Fees (including any applicable Taxes) will be indicated to users prior to posting any Material Information. SIGOOK reserves the right to change the Service Fees at any time, with adequate advance notice.',
        'Employers, Members and/or third parties are responsible for paying any Service Fees that you owe to SIGOOK. Except as otherwise provided on the SIGOOK Platform, Service Fees are non-refundable.',
      ],
    ),
    LegalSection(
      title: 'Taxes',
      paragraphs: [
        'As a Member and/or Employer, you are solely responsible for determining your obligations to report, collect, and remit any applicable taxes due to the respective government authorities.',
        'Tax regulations may require us to collect appropriate Tax information from users. If a user fails to provide us with the required documentation under applicable law, we reserve the right to cancel this Agreement.',
      ],
    ),
    LegalSection(
      title: 'Indemnification',
      paragraphs: [
        'You agree to indemnify, defend, and hold SIGOOK, its parents, subsidiaries, affiliates, officers, directors, agents, and employees harmless from any claims or demands of any third party, including attorneys\' fees and legal fees, resulting from or arising out of:',
        '• Your use of the SIGOOK Platform.',
        '• Your Material Information.',
        '• Your violation of any terms and conditions of this notice.',
      ],
    ),
    LegalSection(
      title: 'Disclaimer',
      paragraphs: [
        'You acknowledge and accept that:',
        '(a) You assume all risks related to or resulting from your usage, viewing, or access of the SIGOOK Platform. The SIGOOK Platform is provided on an "as is" and "as available" basis.',
        '(b) SIGOOK expressly disclaims all warranties of any kind, either express or implied, including implied warranties of merchantability, fitness for a particular purpose, and non-infringement.',
        '(c) SIGOOK expressly disclaims all warranties that the SIGOOK Platform will be error-free, virus-free, uninterrupted, secure, or available at all times; that it will meet your requirements; or that any submitted Material Information will be reliable, accurate, complete, or valid.',
      ],
    ),
    LegalSection(
      title: 'Liability and Limitations of Liability',
      paragraphs: [
        'You agree to assume all risks associated with, arising out of, or resulting from your use of the SIGOOK Platform or any submitted Material Information, including risks of financial loss, physical harm, property damages, dealing with other users, strangers, minors, or foreign nationals.',
        'You further agree to release SIGOOK, its parents, subsidiaries, affiliates, officers, agents, and employees harmless from all claims, demands, damages (direct, indirect, and consequential) of any kind or nature, known or unknown, associated with your usage of the SIGOOK Platform.',
        'In no event shall SIGOOK, its parents, subsidiaries, affiliates, officers, agents, employees, and suppliers be liable for any direct, indirect, consequential, incidental, special damages, or damages for loss of profits, goodwill, revenue, data, or use.',
        'In the event some jurisdictions prohibit the exclusion of certain warranties, SIGOOK\'s aggregate liability for any damages shall not exceed CAD\$15 or \$10 US Dollars.',
      ],
    ),
    LegalSection(
      title: 'Dispute Resolution and Arbitration Agreement',
      paragraphs: [
        'THE PARTIES MUTUALLY AGREE TO WAIVE OUR RESPECTIVE RIGHTS TO RESOLUTION OF DISPUTES IN A COURT OF LAW BY A JUDGE OR JURY AND AGREE TO RESOLVE ANY DISPUTE BY ARBITRATION.',
        'Any dispute relating in any way to your use of SIGOOK\'s website, apps, and online services shall be submitted to confidential arbitration in the State of Florida, United States, under the rules of the American Arbitration Association ("AAA") in accordance with the AAA\'s Consumer Arbitration Rules.',
        'The arbitrator\'s award shall be binding and may be entered as a judgment in any court of competent jurisdiction.',
        'CLASS ACTION WAIVER: Each of us expressly agree that any dispute must be brought in the respective party\'s individual capacity, and not as a plaintiff or class member in any purported class, collective, representative, or similar proceeding.',
        'WAIVER OF JURY TRIAL: IF FOR ANY REASON A CLAIM PROCEEDS IN COURT, YOU AND WE AGREE THAT THERE WILL NOT BE A JURY TRIAL. YOU AND WE UNCONDITIONALLY WAIVE ANY RIGHT TO TRIAL BY JURY IN ANY DISPUTE THAT RELATES TO OR ARISES OUT OF THESE TERMS OF USE.',
      ],
    ),
    LegalSection(
      title: 'Termination',
      paragraphs: [
        'SIGOOK may terminate or suspend your access to the SIGOOK Platform at any time, with or without cause or notice, including if we believe that you have violated or acted inconsistently with the letter or spirit of the Terms.',
        'Upon any such termination or suspension: (a) your right to access and use the SIGOOK Platform will immediately cease; (b) SIGOOK may immediately deactivate or delete your username, password and SIGOOK account; (c) SIGOOK will be under no obligation to maintain or provide you with access to any materials associated with your SIGOOK Account.',
      ],
    ),
    LegalSection(
      title: 'Non-Discrimination Policy',
      paragraphs: [
        'SIGOOK is committed to creating a world where people from every background feel more included and respected. Our commitment to these principles enables every user of our community to feel welcome on the SIGOOK Platform.',
        'We will not discriminate against you for exercising any of your rights stated herein, including by:',
        '• Denying our Services to you.',
        '• Charging different prices for services.',
        '• Providing a different level or quality of our Services.',
        '• Suggesting that you will receive a different price or level of services.',
      ],
    ),
    LegalSection(
      title: 'Notice, Invalidity & Security',
      paragraphs: [
        'Notices: Unless specified otherwise, any notices or other communications permitted or required under this Agreement will be provided electronically by SIGOOK via email or mail.',
        'Invalidity: If any part of the Terms of Service is unenforceable, the enforceability of any other part will not be affected and all other clauses will remain in full force and effect.',
        'Security: SIGOOK is committed to securing and protecting from unauthorized access your Personally Identifiable Information. We use reasonable measures to help prevent the information from becoming disclosed to those who are not described in this policy.',
        'Waiver: If you breach these conditions and we take no action, we will still be entitled to use our rights and remedies in any other situation where you breach these conditions.',
        'Entire Agreement: The above Terms of Service constitute the entire agreement of the parties and supersede all preceding and contemporaneous agreements between you and SIGOOK.',
      ],
    ),
    LegalSection(
      title: 'Equal Employment Opportunity',
      paragraphs: [
        'EEO is the Law: An Equal Opportunity Employer. All qualified applicants will receive consideration for employment without regard to race, color, religion, sex, gender identity, national origin, or protected veteran status and will not be discriminated against on the basis of disability.',
        'SIGOOK is firmly committed to creating a climate where the different perspectives that diversity brings to the business are valued. Attracting and developing a diverse workforce that reflects the communities we serve is at the foundation of this goal.',
        'SIGOOK is an equal opportunity employer, and it is a continuing policy of SIGOOK to afford equal employment opportunities in all aspects of employment to all individuals without regard to race, color, religion, sex or gender (including pregnancy), gender identity or expression, national origin, ancestry, sexual orientation, marital status, age, physical or mental disability, military or veteran status, citizenship or immigration status, genetic information, and any other characteristic protected by applicable law.',
      ],
    ),
    LegalSection(
      title: 'Americans with Disabilities Act',
      paragraphs: [
        'Pursuant to the Americans with Disabilities Act (ADA), SIGOOK will endeavour to provide reasonable accommodation to qualified individuals with known disabilities upon request in order to enable them to apply or perform the essential functions of their job, so long as the accommodation does not impose an undue hardship.',
        'Some examples of potential accommodations include providing assistive devices, reassignment to a different position, and/or finite periods of leave. Each situation will be analyzed individually.',
        'An applicant or employee who believes they require an accommodation should contact the Human Resources Department at appmanager@sigook.com to request accommodation.',
        'SIGOOK does not deny employment to or discriminate against individuals with disabilities, and it will not retaliate against individuals for requesting an accommodation.',
      ],
    ),
  ];

  @override
  Widget build(BuildContext context) {
    return const LegalPage(
      title: 'Terms & Conditions',
      lastUpdated: 'February 01, 2026',
      sections: _sections,
    );
  }
}
