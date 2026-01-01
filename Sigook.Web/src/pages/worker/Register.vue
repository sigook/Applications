<template>
  <div class="white-container-mobile">
    <b-loading v-model="isLoading"></b-loading>
    <form class="form-md" @submit.prevent="validateForm">
      <b-steps animated mobile-mode="compact">
        <b-step-item step="1" label="Basic">
          <h1 class="title has-text-centered">Basic Information</h1>
          <div class="container-flex">
            <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
              <div class=" container-image margin-10-auto">
                <upload-image @imageSelected="(profileImg) => saveImage(profileImg)" :edited-image="worker.profileImage"
                  @onUpload="() => subscribe('file')" @finishUpload="() => unsubscribe()"
                  class="upload-image-spacing" />
                <p class="fz-2">
                  <i>Please upload a photo taken in front of a plain white or off-white background</i>
                </p>
              </div>
            </div>
            <div class="col-sm-12 col-md-6 col-lg-3 col-padding">
              <b-field :type="errors.has('name') ? 'is-danger' : ''"
                :message="errors.has('name') ? errors.first('name') : ''">
                <template #label>
                  {{ $t('WorkerName') }} <span class="has-text-danger">*</span>
                </template>
                <b-input type="text" v-model="worker.firstName" name="name" v-validate="'required|max:20|min:2'" />
              </b-field>
            </div>
            <div class="col-sm-12 col-md-6 col-lg-3 col-padding">
              <b-field :type="errors.has('lastname') ? 'is-danger' : ''"
                :message="errors.has('lastname') ? errors.first('lastname') : ''">
                <template #label>
                  {{ $t('WorkerLastName') }} <span class="has-text-danger">*</span>
                </template>
                <b-input type="text" v-model="worker.lastName" name="lastname" v-validate="'required|max:20|min:2'" />
              </b-field>
            </div>
            <div class="col-sm-12 col-md-6 col-lg-3 col-padding">
              <b-field :type="errors.has('birthday') ? 'is-danger' : ''"
                :message="errors.has('birthday') ? errors.first('birthday') : ''">
                <template #label>
                  {{ $t('WorkerBirthday') }} <span class="has-text-danger">*</span>
                </template>
                <b-datepicker v-model="worker.birthDay" name="birthday" :focused-date="disabledDates"
                  :max-date="disabledDates" v-validate="'required'">
                </b-datepicker>
              </b-field>
            </div>
            <div class="col-sm-12 col-md-6 col-lg-3 col-padding">
              <b-field :type="errors.has('gender') ? 'is-danger' : ''"
                :message="errors.has('gender') ? errors.first('gender') : ''">
                <template #label>
                  {{ $t('WorkerGender') }} <span class="has-text-danger">*</span>
                </template>
                <b-select v-model="gender" name="gender" v-validate="'required'" expanded>
                  <option v-for="item in genders" v-bind:key="item.id" :value="item.id">
                    {{ item.value }}
                  </option>
                </b-select>
              </b-field>
            </div>
          </div>
          <address-component ref="addressComponent" :model="worker.location" @isLoading="(value) => isLoading = value"
            @isCanada="isCanadaSelected($event)" />
          <div class="container-flex">
            <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
              <phone-input ref="phoneComponent" :required="true" model="Mobile Number"
                :defaultValue="worker.mobileNumber" @formattedPhone="(phone) => (worker.mobileNumber = phone)" />
            </div>
          </div>
        </b-step-item>
        <b-step-item step="2" label="Preferences" :visible="!isLogin">
          <h1 class="title has-text-centered">Preferences</h1>
          <div class="container-flex">
            <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
              <b-field :label="$t('WorkerAvailability')">
                <div class="container-flex">
                  <div class="col-sm-12 col-md-6 col-lg-4 col-padding" v-for="item in availabilities"
                    v-bind:key="item.id">
                    <b-checkbox v-model="worker.availabilities" :native-value="item">
                      {{ item.value }}
                    </b-checkbox>
                  </div>
                </div>
              </b-field>
            </div>
            <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
              <b-field :label="$t('WorkerTime')">
                <div class="container-flex">
                  <div class="col-sm-12 col-md-6 col-lg-6 col-padding" v-for="time in availabilityTimes"
                    v-bind:key="time.id">
                    <b-checkbox v-model="worker.availabilityTimes" :native-value="time">
                      {{ time.value }}
                    </b-checkbox>
                  </div>
                </div>
              </b-field>
            </div>
            <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
              <b-field :label="$t('WorkerAvailableDays')">
                <div class="container-flex">
                  <div class="col-sm-12 col-md-6 col-lg-3 col-padding">
                    <b-checkbox v-model="allDaysSelected" @input="changeDaysSelected">
                      All Days
                    </b-checkbox>
                  </div>
                  <div class="col-sm-12 col-md-6 col-lg-3 col-padding" v-for="day in days" v-bind:key="day.id">
                    <b-checkbox v-model="worker.availabilityDays" :native-value="day" @input="changeAllDays">
                      {{ day.value }}
                    </b-checkbox>
                  </div>
                </div>
              </b-field>
            </div>
            <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
              <b-field :label="$t('WorkerYouCanLift')">
                <b-select v-model="worker.lift" placeholder="Select option" expanded>
                  <option v-for="item in lifts" :value="item" v-bind:key="item.id">
                    {{ item.value }}
                  </option>
                </b-select>
              </b-field>
            </div>
            <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
              <b-field :label="$t('WorkerHasVehicle')">
                <b-switch v-model="worker.hasVehicle" :true-value="true" :false-value="false">
                  {{ worker.hasVehicle ? $t("Yes") : $t("No") }}
                </b-switch>
              </b-field>
            </div>
            <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
              <b-field :label="$t('WorkerLanguages')">
                <b-taginput v-model="worker.languages" autocomplete :data="filteredLanguages" open-on-focus
                  field="value" icon="label" placeholder="Select Languages" @typing="getFilteredLanguages">
                </b-taginput>
              </b-field>
            </div>
            <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
              <b-field :label="$t('WorkerSkills')">
                <b-taginput v-model="worker.skills" autocomplete :data="filteredSkills" open-on-focus field="skill"
                  icon="label" placeholder="Select or Add Skills" :maxlength="20" allow-new @typing="getFilteredSkills"
                  :create-tag="addSkill">
                </b-taginput>
              </b-field>
              <span v-show="errors.has('workerSkills')" class="help is-danger no-margin">
                {{ errors.first("workerSkills") }}
              </span>
            </div>
          </div>
        </b-step-item>
        <b-step-item :step="isLogin ? 2 : 3" label="Documents">
          <h1 class="title has-text-centered">Documents</h1>
          <div class="container-flex">
            <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
              <div class="container-flex document-section-header">
                <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
                  <label class="fz1 fw-600 section-label">Documents <span class="has-text-danger">*</span></label>
                </div>
                <div class="col-sm-12 col-md-6 col-lg-6 col-padding upload-button-container">
                  <b-field class="file is-primary upload-field" :class="{
                    'has-name': !!selectedDocumentFile,
                    'upload-disabled': worker.identificationType1File && worker.identificationType2File
                  }">
                    <b-upload v-model="selectedDocumentFile" accept=".pdf,.jpeg,.jpg,.png,.gif,.doc,.docx,.xls,.xlsx"
                      @input="handleIdentificationUpload"
                      :disabled="worker.identificationType1File && worker.identificationType2File" :loading="isLoading"
                      class="file-label" rounded>
                      <span class="file-cta">
                        <b-icon class="file-icon" icon="upload"></b-icon>
                        <span class="file-label">
                          {{ selectedDocumentFile ? selectedDocumentFile.name : $t("AddFile") }}
                        </span>
                      </span>
                    </b-upload>
                  </b-field>
                </div>
              </div>
              <div class="container-files">
                <div class="col-12 col-padding" v-if="worker.identificationType1File">
                  <div class="document-card">
                    <div class="container-flex document-card-header">
                      <div class="col-10 no-padding">
                        <div class="document-icon-title">
                          <b-icon icon="file-document" size="is-small" class="document-icon"></b-icon>
                          <h4 class="fw-600 document-filename">
                            {{ worker.identificationType1File.fileName | filename }}
                          </h4>
                        </div>
                      </div>
                      <div class="col-2 document-delete-container no-padding">
                        <b-tooltip label="Delete" type="is-dark" position="is-top">
                          <b-button type="is-danger" size="is-small" icon-left="delete" outlined
                            @click="deleteDocument(worker.identificationType1File)">
                          </b-button>
                        </b-tooltip>
                      </div>
                    </div>
                    <div class="container-flex">
                      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
                        <b-field :type="errors.has('identificationType1') ? 'is-danger' : ''"
                          :message="errors.has('identificationType1') ? errors.first('identificationType1') : ''">
                          <template #label>
                            Identification Type <span class="has-text-danger">*</span>
                          </template>
                          <b-select v-model="worker.identificationType1" name="identificationType1"
                            v-validate="{ required: true }" placeholder="Select identification type" expanded>
                            <option v-for="(type, index) in identificationTypes" :value="type"
                              :disabled="type === worker.identificationType2"
                              v-bind:key="'identificationType1' + index">
                              {{ type.value }}
                            </option>
                          </b-select>
                        </b-field>
                      </div>
                      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
                        <b-field :type="errors.has('identificationNumber1') ? 'is-danger' : ''"
                          :message="errors.has('identificationNumber1') ? errors.first('identificationNumber1') : ''">
                          <template #label>
                            Identification Number <span class="has-text-danger">*</span>
                          </template>
                          <b-input type="text" :placeholder="$t('IdentificationNumber')"
                            v-model="worker.identificationNumber1" name="identificationNumber1"
                            v-validate="'required|max:15|min:5'" />
                        </b-field>
                      </div>
                    </div>
                  </div>
                </div>
                <div class="col-12 col-padding" v-if="worker.identificationType2File">
                  <div class="document-card">
                    <div class="container-flex document-card-header">
                      <div class="col-10 no-padding">
                        <div class="document-icon-title">
                          <b-icon icon="file-document" size="is-small" class="document-icon"></b-icon>
                          <h4 class="fw-600 document-filename">
                            {{ worker.identificationType2File.fileName | filename }}
                          </h4>
                        </div>
                      </div>
                      <div class="col-2 document-delete-container no-padding">
                        <b-tooltip label="Delete" type="is-dark" position="is-top">
                          <b-button type="is-danger" size="is-small" icon-left="delete" outlined
                            @click="deleteDocument(worker.identificationType2File)">
                          </b-button>
                        </b-tooltip>
                      </div>
                    </div>
                    <div class="container-flex">
                      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
                        <b-field :type="errors.has('identificationType2') ? 'is-danger' : ''"
                          :message="errors.has('identificationType2') ? errors.first('identificationType2') : ''">
                          <template #label>
                            Identification Type <span class="has-text-danger">*</span>
                          </template>
                          <b-select v-model="worker.identificationType2" name="identificationType2"
                            v-validate="{ required: true }" placeholder="Select identification type" expanded>
                            <option v-for="(type, index) in identificationTypes" :value="type"
                              :disabled="type === worker.identificationType1"
                              v-bind:key="'identificationType2' + index">
                              {{ type.value }}
                            </option>
                          </b-select>
                        </b-field>
                      </div>
                      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
                        <b-field :type="errors.has('identificationNumber2') ? 'is-danger' : ''"
                          :message="errors.has('identificationNumber2') ? errors.first('identificationNumber2') : ''">
                          <template #label>
                            Identification Number <span class="has-text-danger">*</span>
                          </template>
                          <b-input type="text" :placeholder="$t('IdentificationNumber')"
                            v-model="worker.identificationNumber2" name="identificationNumber2"
                            v-validate="'required|max:15|min:5'" />
                        </b-field>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
              <span v-show="documentsError" class="help is-danger">
                At least one identification document is required
              </span>
            </div>
            <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
              <div class="container-flex document-section-header">
                <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
                  <label class="fz1 fw-600 section-label">{{ $t("WorkerLicenses") }}</label>
                </div>
                <div class="col-sm-12 col-md-6 col-lg-6 col-padding upload-button-container">
                  <b-field class="file is-primary upload-field" :class="{ 'has-name': !!selectedLicenseFile }">
                    <b-upload v-model="selectedLicenseFile" accept=".pdf,.jpeg,.jpg,.png,.gif,.doc,.docx,.xls,.xlsx"
                      @input="handleLicenseUpload" :loading="isLoading" class="file-label" rounded>
                      <span class="file-cta">
                        <b-icon class="file-icon" icon="upload"></b-icon>
                        <span class="file-label">
                          {{ selectedLicenseFile ? selectedLicenseFile.name : $t("AddFile") }}
                        </span>
                      </span>
                    </b-upload>
                  </b-field>
                </div>
              </div>
              <div class="container-files">
                <div class="col-12 col-padding" v-for="(item, index) in worker.licenses"
                  v-bind:key="'licences' + index">
                  <div class="document-card">
                    <div class="container-flex document-card-header">
                      <div class="col-10 no-padding">
                        <div class="document-icon-title">
                          <b-icon icon="certificate" size="is-small" class="document-icon"></b-icon>
                          <h4 class="fw-600 document-filename">
                            {{ item.license.fileName | filename }}
                          </h4>
                        </div>
                      </div>
                      <div class="col-2 document-delete-container no-padding">
                        <b-tooltip label="Delete" type="is-dark" position="is-top">
                          <b-button type="is-danger" size="is-small" icon-left="delete" outlined
                            @click="deleteLicense(index)">
                          </b-button>
                        </b-tooltip>
                      </div>
                    </div>
                    <div class="container-flex">
                      <div class="col-sm-12 col-md-8 col-lg-8 col-padding">
                        <b-field :type="errors.has('description' + index) ? 'is-danger' : ''"
                          :message="errors.has('description' + index) ? errors.first('description' + index) : ''">
                          <template #label>
                            Description <span class="has-text-danger">*</span>
                          </template>
                          <b-input type="text" :placeholder="$t('Description')" v-model="item.license.description"
                            :name="'description' + index" v-validate="{
                              required: true,
                              max: 100,
                              min: 1,
                              regex: alphaNumericSpaces
                            }" />
                        </b-field>
                      </div>
                      <div class="col-sm-12 col-md-4 col-lg-4 col-padding">
                        <b-field :type="errors.has('licenseExpires' + index) ? 'is-danger' : ''"
                          :message="errors.has('licenseExpires' + index) ? errors.first('licenseExpires' + index) : ''">
                          <template #label>
                            Expires In <span class="has-text-danger">*</span>
                          </template>
                          <b-datepicker v-model="item.expires" :name="'licenseExpires' + index" v-validate="'required'">
                          </b-datepicker>
                        </b-field>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
            <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
              <div class="container-flex document-section-header">
                <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
                  <label class="fz1 fw-600 section-label">{{ $t("WorkerCertificates") }}</label>
                </div>
                <div class="col-sm-12 col-md-6 col-lg-6 col-padding upload-button-container">
                  <b-field class="file is-primary upload-field" :class="{ 'has-name': !!selectedCertificateFile }">
                    <b-upload v-model="selectedCertificateFile" accept=".pdf,.jpeg,.jpg,.png,.gif,.doc,.docx,.xls,.xlsx"
                      @input="handleCertificateUpload" :loading="isLoading" class="file-label" rounded>
                      <span class="file-cta">
                        <b-icon class="file-icon" icon="upload"></b-icon>
                        <span class="file-label">
                          {{ selectedCertificateFile ? selectedCertificateFile.name : $t("AddFile") }}
                        </span>
                      </span>
                    </b-upload>
                  </b-field>
                </div>
              </div>
              <div class="container-files">
                <div class="col-12 col-padding" v-for="(item, index) in worker.certificates"
                  v-bind:key="'certificates' + index">
                  <div class="document-card">
                    <div class="container-flex document-card-header">
                      <div class="col-10 no-padding">
                        <div class="document-icon-title">
                          <b-icon icon="card-account-details" size="is-small" class="document-icon"></b-icon>
                          <h4 class="fw-600 document-filename">{{ item.fileName | filename }}</h4>
                        </div>
                      </div>
                      <div class="col-2 document-delete-container no-padding">
                        <b-tooltip label="Delete" type="is-dark" position="is-top">
                          <b-button type="is-danger" size="is-small" icon-left="delete" outlined
                            @click="deleteCertificate(index)">
                          </b-button>
                        </b-tooltip>
                      </div>
                    </div>
                    <b-field :type="errors.has('descriptioncer' + index) ? 'is-danger' : ''"
                      :message="errors.has('descriptioncer' + index) ? errors.first('descriptioncer' + index) : ''"
                      label="Description">
                      <b-input type="text" placeholder="Description" v-model="item.description"
                        :name="'descriptioncer' + index" v-validate="{
                          required: true,
                          max: 100,
                          min: 1,
                          regex: alphaNumericSpaces
                        }" />
                    </b-field>
                  </div>
                </div>
              </div>
            </div>
            <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
              <div class="container-flex document-section-header">
                <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
                  <label class="fz1 fw-600 section-label">{{ $t("Resume") }}</label>
                </div>
                <div class="col-sm-12 col-md-6 col-lg-6 col-padding upload-button-container">
                  <b-field class="file is-primary upload-field" :class="{
                    'has-name': !!selectedResumeFile,
                    'upload-disabled': worker.resume
                  }">
                    <b-upload v-model="selectedResumeFile" accept=".pdf,.jpeg,.jpg,.png,.gif,.doc,.docx,.xls,.xlsx"
                      @input="handleResumeUpload" :disabled="worker.resume ? true : false" :loading="isLoading"
                      class="file-label" rounded>
                      <span class="file-cta">
                        <b-icon class="file-icon" icon="upload"></b-icon>
                        <span class="file-label">
                          {{ selectedResumeFile ? selectedResumeFile.name : $t("AddFile") }}
                        </span>
                      </span>
                    </b-upload>
                  </b-field>
                </div>
              </div>
              <div class="container-files">
                <div class="col-12 col-padding" v-if="worker.resume">
                  <div class="document-card">
                    <div class="container-flex document-card-header">
                      <div class="col-10 no-padding">
                        <div class="document-icon-title">
                          <b-icon icon="file-account" size="is-small" class="document-icon"></b-icon>
                          <h4 class="fw-600 document-filename">
                            {{ worker.resume.fileName | filename }}
                          </h4>
                        </div>
                      </div>
                      <div class="col-2 document-delete-container no-padding">
                        <b-tooltip label="Delete" type="is-dark" position="is-top">
                          <b-button type="is-danger" size="is-small" icon-left="delete" outlined
                            @click="deleteResume()">
                          </b-button>
                        </b-tooltip>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
            <div class="col-sm-12 col-md-12 col-lg-12 col-padding" v-if="isCanada">
              <div class="container-flex document-section-header">
                <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
                  <div>
                    <label class="fz1 fw-600 section-label block-label">WHMIS and Health and Safety Training</label>
                    <i class="fz-2">Complete the training following both links below and upload your certificates</i>
                  </div>
                </div>
                <div class="col-sm-12 col-md-6 col-lg-6 col-padding upload-button-container">
                  <b-field class="file is-primary upload-field" :class="{ 'has-name': !!selectedOtherDocFile }">
                    <b-upload v-model="selectedOtherDocFile" accept=".pdf,.jpeg,.jpg,.png,.gif,.doc,.docx,.xls,.xlsx"
                      @input="handleOtherDocumentUpload" :loading="isLoading" class="file-label" rounded>
                      <span class="file-cta">
                        <b-icon class="file-icon" icon="upload"></b-icon>
                        <span class="file-label">
                          {{ selectedOtherDocFile ? selectedOtherDocFile.name : $t("AddFile") }}
                        </span>
                      </span>
                    </b-upload>
                  </b-field>
                </div>
              </div>
              <div class="col-sm-12 col-md-12 col-lg-12 col-padding canada-links-container">
                <p class="canada-link">
                  <a href="https://aixsafety.com/wp-content/uploads/articulate_uploads/WHS-Apr2025Aix/story.html"
                    target="_blank" class="color-primary fw-600">WHIMS Training</a>
                </p>
                <p>
                  <a href="https://www.labour.gov.on.ca/english/hs/elearn/worker/foursteps.php" target="_blank"
                    class="color-primary fw-600">HS BOOKLET</a>
                </p>
              </div>
              <div class="container-files">
                <div class="col-12 col-padding" v-for="(item, index) in worker.otherDocuments"
                  v-bind:key="'otherDocument' + index">
                  <div class="document-card">
                    <div class="container-flex document-card-header">
                      <div class="col-10 no-padding">
                        <div class="document-icon-title">
                          <b-icon icon="folder-open" size="is-small" class="document-icon"></b-icon>
                          <h4 class="fw-600 document-filename">{{ item.fileName | filename }}</h4>
                        </div>
                      </div>
                      <div class="col-2 document-delete-container no-padding">
                        <b-tooltip label="Delete" type="is-dark" position="is-top">
                          <b-button type="is-danger" size="is-small" icon-left="delete" outlined
                            @click="deleteOtherDocument(index)">
                          </b-button>
                        </b-tooltip>
                      </div>
                    </div>
                    <b-field :type="errors.has('descriptionOther' + index) ? 'is-danger' : ''"
                      :message="errors.has('descriptionOther' + index) ? errors.first('descriptionOther' + index) : ''"
                      label="Description">
                      <b-input type="text" placeholder="Description" v-model="item.description"
                        :name="'descriptionOther' + index" v-validate="{
                          required: true,
                          max: 100,
                          min: 1,
                          regex: alphaNumericSpaces
                        }" />
                    </b-field>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </b-step-item>
        <b-step-item :step="isLogin ? 3 : 4" label="Account">
          <h1 class="title has-text-centered">Account</h1>
          <div class="container-flex">
            <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
              <b-field :type="errors.has('email') ? 'is-danger' : ''"
                :message="errors.has('email') ? errors.first('email') : ''">
                <template #label>
                  {{ $t('Email') }} <span class="has-text-danger">*</span>
                </template>
                <b-input type="email" v-model="worker.email" name="email" v-validate="'required|max:50|min:6|email'"
                  :class="{ 'is-danger': errors.has('email') }" />
              </b-field>
            </div>
            <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
              <b-field :type="errors.has('password') ? 'is-danger' : ''"
                :message="errors.has('password') ? errors.first('password') : ''">
                <template #label>
                  {{ $t('Password') }} <span class="has-text-danger">*</span>
                </template>
                <b-input type="password" v-model="worker.password" name="password" v-validate="'required|max:100|min:6'"
                  data-vv-as="password" ref="password" />
              </b-field>
            </div>
            <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
              <b-field :type="errors.has('confirmPassword') ? 'is-danger' : ''"
                :message="errors.has('confirmPassword') ? errors.first('confirmPassword') : ''">
                <template #label>
                  {{ $t('ConfirmPassword') }} <span class="has-text-danger">*</span>
                </template>
                <b-input type="password" v-model="worker.confirmPassword" name="confirmPassword"
                  v-validate="'required|confirmed:password'" />
              </b-field>
            </div>
            <div class="col-sm-12 col-md-12 col-lg-12 col-padding" v-if="!isLogin">
              <b-field>
                <b-checkbox v-model="worker.agreeTermsAndConditions" name="agree terms" v-validate="{
                  required: !isLogin
                }">
                  {{ $t("IAgreeAll2job") }}
                  <router-link to="/terms-and-conditions" target="_blank">
                    <u class="color-primary">{{ $t("TermsAndConditions") }}</u>
                  </router-link>
                  &
                  <router-link to="/privacy-policy" target="_blank">
                    <u class="color-primary">{{ $t("PrivacyPolicy") }}.</u>
                  </router-link>
                </b-checkbox>
              </b-field>
              <span v-show="errors.has('agree terms')" class="help is-danger no-margin">
                {{ $t("YouMustAcceptTheTermsAndConditionsToContinue") }}.
              </span>
            </div>
            <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
              <b-button type="is-primary" native-type="submit">
                {{ $t("Register") }}
              </b-button>
            </div>
          </div>
        </b-step-item>
      </b-steps>
    </form>
  </div>
</template>

<script>
import dayjs from "dayjs";
import createWorkerMixin from "@/mixins/createWorkerMixin";
import confirmationAlert from "../../mixins/confirmationAlert";
import updateMixin from "../../mixins/uploadFiles";
import multipartUploadMixin from "../../mixins/multipartUploadMixin";

export default {
  components: {
    uploadImage: () => import("../../components/PreviewImage"),
    addressComponent: () => import("../../components/Address"),
    phoneInput: () => import("../../components/PhoneInput"),
  },
  data() {
    return {
      showWorkInformationTab: true,
      alphaNumericSpaces: /^[-_ a-zA-Z0-9]+$/,
      disableStartDate: null,
      submitted: false,
      isLoading: true,
      gender: null,
      disabledDates: null,
      isCanada: false,
      selectedDocumentFile: null,
      selectedLicenseFile: null,
      selectedCertificateFile: null,
      selectedResumeFile: null,
      selectedOtherDocFile: null,
      documentsError: null,
      fileObjects: {
        profileImage: null,
        identificationType1: null,
        identificationType2: null,
        licenses: [],
        certificates: [],
        resume: null,
        otherDocuments: []
      }
    };
  },
  methods: {
    async validateForm() {
      const mainFormValid = await this.$validator.validateAll();
      const addressValid = await this.$refs.addressComponent.validateAddress();
      const phoneValid = await this.$refs.phoneComponent.validatePhone();

      this.documentsError = !this.worker.identificationType1File;

      if (mainFormValid && addressValid && phoneValid && !this.documentsError) {
        this.registerWorker();
        return;
      } else {
        this.showAlertError(this.$t("PleaseVerifyThatTheFieldsAreCorrect"));
      }
    },
    async registerWorker() {
      this.isLoading = true;
      this.worker.gender = { id: this.gender };

      console.log('===== DEBUG: WORKER OBJECT =====');
      console.log('Licenses:', JSON.stringify(this.worker.licenses, null, 2));
      console.log('Certificates:', JSON.stringify(this.worker.certificates, null, 2));
      console.log('Resume:', JSON.stringify(this.worker.resume, null, 2));
      console.log('Other Docs:', JSON.stringify(this.worker.otherDocuments, null, 2));
      console.log('================================');

      try {
        const formData = await this.createMultipartFormData(this.worker, this.fileObjects);

        console.log('===== DEBUG: FORMDATA CONTENTS =====');
        const dataField = formData.get('data');
        console.log('Data field:', dataField);
        const parsedData = JSON.parse(dataField);
        console.log('Licenses:', parsedData.licenses);
        console.log('Certificates:', parsedData.certificates);
        console.log('Resume:', parsedData.resume);
        console.log('Other Documents:', parsedData.otherDocuments);
        console.log('====================================');

        const action = this.isLogin ? `agency/createWorker` : `worker/registerWorker`

        const id = await this.$store.dispatch(action, formData);
        this.isLoading = false;
        this.showAlertSuccess(this.$t("YourAccountHasBeenCreated"));
        const route = this.isLogin ? `/agency-workers/worker/${id}` : '/home'
        this.$router.push(route);
      } catch (error) {
        this.isLoading = false;
        this.showAlertError(error.data);
      }
    },
    validateDocumentFile(file, maxSizeKB = 15500) {
      if (!file) return false;

      if (file.size / 1024 > maxSizeKB) {
        this.showAlertError('File exceeds 15MB limit');
        return false;
      }

      return true;
    },
    handleIdentificationUpload(file) {
      if (this.validateDocumentFile(file)) {
        this.addDocument(file);
        this.selectedDocumentFile = null;
      }
    },
    handleLicenseUpload(file) {
      if (this.validateDocumentFile(file)) {
        this.addLicense(file);
        this.selectedLicenseFile = null;
      }
    },
    handleCertificateUpload(file) {
      if (this.validateDocumentFile(file)) {
        this.addCertificate(file);
        this.selectedCertificateFile = null;
      }
    },
    handleResumeUpload(file) {
      if (this.validateDocumentFile(file)) {
        this.addResume(file);
        this.selectedResumeFile = null;
      }
    },
    handleOtherDocumentUpload(file) {
      if (this.validateDocumentFile(file)) {
        this.addOtherDocument(file);
        this.selectedOtherDocFile = null;
      }
    },
    addDocument(file) {
      if (!this.worker.identificationType1File) {
        this.fileObjects.identificationType1 = file;
        this.worker.identificationType1File = {
          fileName: file.name,
          description: "",
        };
        this.worker.identificationType1 = null;
        this.worker.identificationNumber1 = null;
      } else {
        this.fileObjects.identificationType2 = file;
        this.worker.identificationType2File = {
          fileName: file.name,
          description: "",
        };
        this.worker.identificationType2 = null;
        this.worker.identificationNumber2 = null;
      }
      this.documentsError = false;
    },
    deleteDocument(file) {
      const isFile1 =
        this.worker.identificationType1File.fileName === file.fileName;
      if (isFile1 && this.worker.identificationType2File) {
        this.worker.identificationType1File = {
          ...this.worker.identificationType2File,
        };
        this.fileObjects.identificationType1 = this.fileObjects.identificationType2;
        this.worker.identificationType1 = this.worker.identificationType2;
        this.worker.identificationNumber1 = this.worker.identificationNumber2;
        this.fileObjects.identificationType2 = null;
        this.worker.identificationType2File = null;
        this.worker.identificationType2 = null;
        this.worker.identificationNumber2 = null;
      } else if (isFile1) {
        this.fileObjects.identificationType1 = null;
        this.worker.identificationType1File = null;
        this.worker.identificationType1 = null;
        this.worker.identificationNumber1 = null;
      } else {
        this.fileObjects.identificationType2 = null;
        this.worker.identificationType2File = null;
        this.worker.identificationType2 = null;
        this.worker.identificationNumber2 = null;
      }
    },
    addLicense(file) {
      this.fileObjects.licenses.push(file);
      this.worker.licenses.push({
        license: {
          fileName: file.name,
          description: "",
        },
      });
    },
    deleteLicense(index) {
      this.fileObjects.licenses.splice(index, 1);
      this.worker.licenses.splice(index, 1);
    },
    addCertificate(file) {
      this.fileObjects.certificates.push(file);
      this.worker.certificates.push({
        fileName: file.name,
        description: "",
      });
    },
    deleteCertificate(index) {
      this.fileObjects.certificates.splice(index, 1);
      this.worker.certificates.splice(index, 1);
    },
    addResume(file) {
      this.fileObjects.resume = file;
      this.worker.resume = {
        fileName: file.name
      };
    },
    deleteResume() {
      this.fileObjects.resume = null;
      this.worker.resume = null;
    },
    addOtherDocument(file) {
      this.fileObjects.otherDocuments.push(file);
      this.worker.otherDocuments.push({
        fileName: file.name,
        description: ""
      });
    },
    deleteOtherDocument(index) {
      this.fileObjects.otherDocuments.splice(index, 1);
      this.worker.otherDocuments.splice(index, 1);
    },
    saveImage(image) {
      this.fileObjects.profileImage = image;
      this.worker.profileImage.fileName = image.name;
    },
    addSkill(skill) {
      if (!skill.skill) {
        skill = { skill };
        this.skills.push(skill);
      }
      return skill;
    },
    isCanadaSelected(value) {
      this.isCanada = value;
      this.isLoading = true;
      if (value === false && this.worker.otherDocuments.length > 0) {
        this.fileObjects.otherDocuments = [];
        this.worker.otherDocuments = [];
      }
      this.isLoading = false;
    }
  },
  mixins: [
    createWorkerMixin,
    confirmationAlert,
    updateMixin,
    multipartUploadMixin
  ],
  created() {
    this.$store.dispatch("getCurrentDate").then((response) => {
      this.disableStartDate = response;
      this.disabledDates = dayjs(response)
        .subtract(18, "years")
        .toDate();
    });
  },
  computed: {
    isLogin() {
      return this.$store.state.security.user ? true : false;
    },
  }
};
</script>

<style lang="scss" scoped>
.document-section-header {
  align-items: center;
  margin-bottom: 15px;
}

.section-label {
  margin-bottom: 0;
}

.upload-button-container {
  text-align: right;
}

.upload-field {
  margin-bottom: 0;
}

.document-card {
  background: #fafafa;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  padding: 20px;
  margin-bottom: 15px;
}

.document-card-header {
  align-items: center;
  margin-bottom: 15px;
}

.document-icon-title {
  display: flex;
  align-items: center;
}

.document-icon {
  margin-right: 10px;
  color: #7957d5;
}

.document-filename {
  margin: 0;
  color: #363636;
}

.document-delete-container {
  text-align: right;
}

.canada-links-container {
  margin-bottom: 15px;
}

.canada-link {
  margin-bottom: 5px;
}

.upload-image-spacing {
  margin-top: 23px;
  margin-bottom: 0;
}

.no-padding {
  padding: 0;
}

.block-label {
  display: block;
}

.upload-disabled {
  opacity: 0.5;
  cursor: not-allowed;
  pointer-events: none;
}
</style>
