import { RECIPE_PHOTO_UPLOAD_SUCCESS, RECIPE_PHOTO_UPLOAD_FAIL, PHOTO_DELETED } from '../actions/types';

const initialState = {
	// userPhotos: [],
	postPhotos: [],
	loading: true,
	error: {}
};

export default function(state = initialState, action) {
	const { type, payload } = action;

	switch (type) {
		case RECIPE_PHOTO_UPLOAD_SUCCESS:
			return {
				...state,
				loading: false,
				postPhotos: [ payload, ...state.postPhotos ]
			};
		case RECIPE_PHOTO_UPLOAD_FAIL:
			return {
				...state,
				error: payload,
				loading: false
			};
		default:
			return state;
	}
}
